// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Pagination
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.CosmosElements;
    using Microsoft.Azure.Cosmos.Diagnostics;
    using Microsoft.Azure.Cosmos.Json;
    using Microsoft.Azure.Cosmos.Query.Core;
    using Microsoft.Azure.Cosmos.Query.Core.Monads;
    using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
    using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
    using Microsoft.Azure.Cosmos.ReadFeed.Pagination;
    using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
    using Microsoft.Azure.Cosmos.Tracing;
    using Microsoft.Azure.Documents;

    internal sealed class NetworkAttachedDocumentContainer : IMonadicDocumentContainer
    {
        private readonly ContainerInternal container;
        private readonly CosmosQueryClient cosmosQueryClient;
        private readonly QueryRequestOptions queryRequestOptions;
        private readonly ChangeFeedRequestOptions changeFeedRequestOptions;
        private readonly string resourceLink;
        private readonly ResourceType resourceType;
        private readonly Guid correlatedActivityId;

        public NetworkAttachedDocumentContainer(
            ContainerInternal container,
            CosmosQueryClient cosmosQueryClient,
            Guid correlatedActivityId,
            QueryRequestOptions queryRequestOptions = null,
            ChangeFeedRequestOptions changeFeedRequestOptions = null,
            string resourceLink = null,
            ResourceType resourceType = ResourceType.Document)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
            this.cosmosQueryClient = cosmosQueryClient ?? throw new ArgumentNullException(nameof(cosmosQueryClient));
            this.queryRequestOptions = queryRequestOptions;
            this.changeFeedRequestOptions = changeFeedRequestOptions;
            this.resourceLink = resourceLink ?? this.container.LinkUri;
            this.resourceType = resourceType;
            this.correlatedActivityId = correlatedActivityId;
        }

        public Task<TryCatch> MonadicSplitAsync(
            FeedRangeInternal feedRange,
            CancellationToken cancellationToken) => Task.FromResult(TryCatch.FromException(new NotSupportedException()));

        public Task<TryCatch> MonadicMergeAsync(
            FeedRangeInternal feedRange1,
            FeedRangeInternal feedRange2,
            CancellationToken cancellationToken) => Task.FromResult(TryCatch.FromException(new NotSupportedException()));

        public async Task<TryCatch<Record>> MonadicCreateItemAsync(
            CosmosObject payload,
            CancellationToken cancellationToken)
        {
            ItemResponse<CosmosObject> tryInsertDocument = await this.container.CreateItemAsync(
                payload,
                cancellationToken: cancellationToken);
            if (tryInsertDocument.StatusCode != HttpStatusCode.Created)
            {
                return TryCatch<Record>.FromException(
                    new CosmosException(
                        message: "Failed to insert document",
                        statusCode: tryInsertDocument.StatusCode,
                        subStatusCode: default,
                        activityId: tryInsertDocument.ActivityId,
                        requestCharge: tryInsertDocument.RequestCharge));
            }

            CosmosObject insertedDocument = tryInsertDocument.Resource;
            string identifier = ((CosmosString)insertedDocument["id"]).Value;
            ResourceId resourceIdentifier = ResourceId.Parse(((CosmosString)insertedDocument["_rid"]).Value);
            long ticks = Number64.ToLong(((CosmosNumber)insertedDocument["_ts"]).Value);

            Record record = new Record(
                resourceIdentifier,
                new DateTime(ticks: ticks, DateTimeKind.Utc),
                identifier,
                insertedDocument);

            return TryCatch<Record>.FromResult(record);
        }

        public Task<TryCatch<Record>> MonadicReadItemAsync(
            CosmosElement partitionKey,
            string identifer,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TryCatch<List<FeedRangeEpk>>> MonadicGetFeedRangesAsync(
            ITrace trace,
            CancellationToken cancellationToken) => this.MonadicGetChildRangeAsync(FeedRangeEpk.FullRange, trace, cancellationToken);

        public async Task<TryCatch<List<FeedRangeEpk>>> MonadicGetChildRangeAsync(
            FeedRangeInternal feedRange,
            ITrace trace,
            CancellationToken cancellationToken)
        {
            try
            {
                ContainerProperties containerProperties = await this.container.ClientContext.GetCachedContainerPropertiesAsync(
                    this.container.LinkUri,
                    trace,
                    cancellationToken);
                List<PartitionKeyRange> overlappingRanges = await this.cosmosQueryClient.GetTargetPartitionKeyRangeByFeedRangeAsync(
                    this.container.LinkUri,
                    await this.container.GetCachedRIDAsync(forceRefresh: false, trace, cancellationToken: cancellationToken),
                    containerProperties.PartitionKey,
                    feedRange,
                    forceRefresh: false,
                    trace);
                return TryCatch<List<FeedRangeEpk>>.FromResult(
                    overlappingRanges.Select(range => new FeedRangeEpk(
                        new Documents.Routing.Range<string>(
                            min: range.MinInclusive,
                            max: range.MaxExclusive,
                            isMinInclusive: true,
                            isMaxInclusive: false))).ToList());
            }
            catch (Exception ex)
            {
                return TryCatch<List<FeedRangeEpk>>.FromException(ex);
            }
        }

        public async Task<TryCatch> MonadicRefreshProviderAsync(
            ITrace trace,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (ITrace refreshTrace = trace.StartChild("Refresh FeedRangeProvider", TraceComponent.Routing, TraceLevel.Info))
            {
                try
                {
                    string resourceId = await this.container.GetCachedRIDAsync(
                        forceRefresh: false,
                        trace: refreshTrace,
                        cancellationToken: cancellationToken);

                    // We can refresh the cache by just getting all the ranges for this container using the force refresh flag
                    _ = await this.cosmosQueryClient.TryGetOverlappingRangesAsync(
                        resourceId,
                        FeedRangeEpk.FullRange.Range,
                        forceRefresh: true);

                    return TryCatch.FromResult();
                }
                catch (Exception ex)
                {
                    return TryCatch.FromException(ex);
                }
            }
        }

        public async Task<TryCatch<ReadFeedPage>> MonadicReadFeedAsync(
            FeedRangeState<ReadFeedState> feedRangeState,
            ReadFeedPaginationOptions readFeedPaginationOptions,
            ITrace trace,
            CancellationToken cancellationToken)
        {
            readFeedPaginationOptions ??= ReadFeedPaginationOptions.Default;

            ResponseMessage responseMessage = await this.container.ClientContext.ProcessResourceOperationStreamAsync(
                resourceUri: this.resourceLink,
                resourceType: this.resourceType,
                operationType: OperationType.ReadFeed,
                requestOptions: this.queryRequestOptions,
                cosmosContainerCore: this.container,
                requestEnricher: request =>
                {
                    // We don't set page size here, since it's already set by the query request options.
                    if (feedRangeState.State is ReadFeedContinuationState readFeedContinuationState)
                    {
                        request.Headers.ContinuationToken = ((CosmosString)readFeedContinuationState.ContinuationToken).Value;
                    }

                    foreach (KeyValuePair<string, string> kvp in readFeedPaginationOptions.AdditionalHeaders)
                    {
                        request.Headers[kvp.Key] = kvp.Value;
                    }
                },
                feedRange: feedRangeState.FeedRange,
                streamPayload: default,
                trace: trace,
                cancellationToken: cancellationToken);

            TryCatch<ReadFeedPage> monadicReadFeedPage;
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                double requestCharge = responseMessage.Headers.RequestCharge;
                string activityId = responseMessage.Headers.ActivityId;
                int itemCount = int.Parse(responseMessage.Headers.ItemCount);
                ReadFeedState state = responseMessage.Headers.ContinuationToken != null ? ReadFeedState.Continuation(CosmosString.Create(responseMessage.Headers.ContinuationToken)) : null;
                Dictionary<string, string> additionalHeaders = GetAdditionalHeaders(
                    responseMessage.Headers.CosmosMessageHeaders,
                    ReadFeedPage.BannedHeaders);

                ReadFeedPage readFeedPage = new ReadFeedPage(
                    responseMessage.Content,
                    requestCharge,
                    itemCount,
                    activityId,
                    additionalHeaders,
                    state);

                monadicReadFeedPage = TryCatch<ReadFeedPage>.FromResult(readFeedPage);
            }
            else
            {
                CosmosException cosmosException = CosmosExceptionFactory.Create(responseMessage);
                monadicReadFeedPage = TryCatch<ReadFeedPage>.FromException(cosmosException);
            }

            return monadicReadFeedPage;
        }

        public async Task<TryCatch<QueryPage>> MonadicQueryAsync(
            SqlQuerySpec sqlQuerySpec,
            FeedRangeState<QueryState> feedRangeState,
            QueryPaginationOptions queryPaginationOptions,
            ITrace trace,
            CancellationToken cancellationToken)
        {
            if (sqlQuerySpec == null)
            {
                throw new ArgumentNullException(nameof(sqlQuerySpec));
            }

            if (queryPaginationOptions == null)
            {
                throw new ArgumentNullException(nameof(queryPaginationOptions));
            }

            if (trace == null)
            {
                throw new ArgumentNullException(nameof(trace));
            }

            QueryRequestOptions queryRequestOptions = this.queryRequestOptions == null ? new QueryRequestOptions() : this.queryRequestOptions;
            AdditionalRequestHeaders additionalRequestHeaders = new AdditionalRequestHeaders(this.correlatedActivityId, isContinuationExpected: false, optimisticDirectExecute: queryPaginationOptions.OptimisticDirectExecute);

            TryCatch<QueryPage> monadicQueryPage = await this.cosmosQueryClient.ExecuteItemQueryAsync(
                this.resourceLink,
                this.resourceType,
                Documents.OperationType.Query,
                feedRangeState.FeedRange,
                queryRequestOptions,
                additionalRequestHeaders,
                sqlQuerySpec,
                feedRangeState.State == null ? null : ((CosmosString)feedRangeState.State.Value).Value,
                queryPaginationOptions.PageSizeLimit ?? int.MaxValue,
                trace,
                cancellationToken);

            return monadicQueryPage;
        }


        public async Task<TryCatch<string>> MonadicGetResourceIdentifierAsync(ITrace trace, CancellationToken cancellationToken)
        {
            try
            {
                string resourceIdentifier = await this.container.GetCachedRIDAsync(forceRefresh: false, trace, cancellationToken);
                return TryCatch<string>.FromResult(resourceIdentifier);
            }
            catch (Exception ex)
            {
                return TryCatch<string>.FromException(ex);
            }
        }

        private static Dictionary<string, string> GetAdditionalHeaders(CosmosMessageHeadersInternal headers, ImmutableHashSet<string> bannedHeaders)
        {
            Dictionary<string, string> additionalHeaders = new Dictionary<string, string>(capacity: headers.Count());
            foreach (string key in headers)
            {
                if (!bannedHeaders.Contains(key))
                {
                    additionalHeaders[key] = headers[key];
                }
            }

            return additionalHeaders;
        }
    }
}
