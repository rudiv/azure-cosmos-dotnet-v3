//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Linq;
    using Microsoft.Azure.Cosmos.Query.Core;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;

    internal partial class DocumentClient : IDisposable, IAuthorizationTokenProvider
    {
        #region Create operation

        /// <summary>
        /// Creates a document as an asychronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentCollectionUri">the URI of the document collection to create the document in.</param>
        /// <param name="document">the document object.</param>
        /// <param name="options">The request options for the request.</param>
        /// <param name="disableAutomaticIdGeneration">Disables the automatic id generation, will throw an exception if id is missing.</param>
        /// <param name="cancellationToken">(Optional) <see cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<Document>> CreateDocumentAsync(Uri documentCollectionUri, object document, Documents.Client.RequestOptions options = null, bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default)
        {
            if (documentCollectionUri == null)
            {
                throw new ArgumentNullException("documentCollectionUri");
            }

            return this.CreateDocumentAsync(documentCollectionUri.OriginalString, document, options, disableAutomaticIdGeneration, cancellationToken);
        }

        /// <summary>
        /// Creates a collection as an asychronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="databaseUri">the URI of the database to create the collection in.</param>
        /// <param name="documentCollection">the Microsoft.Azure.Documents.DocumentCollection object.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionAsync(Uri databaseUri, DocumentCollection documentCollection, Documents.Client.RequestOptions options = null)
        {
            if (databaseUri == null)
            {
                throw new ArgumentNullException("databaseUri");
            }
            return this.CreateDocumentCollectionAsync(databaseUri.OriginalString, documentCollection, options);
        }

        /// <summary>
        /// Creates(if doesn't exist) or gets(if already exists) a collection as an asychronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="databaseUri">the URI of the database to create the collection in.</param>
        /// <param name="documentCollection">The <see cref="DocumentCollection"/> object.</param>
        /// <param name="options">(Optional) Any <see cref="Microsoft.Azure.Documents.Client.RequestOptions"/> you wish to provide when creating a Collection. E.g. RequestOptions.OfferThroughput = 400. </param>
        /// <returns>The <see cref="DocumentCollection"/> that was created contained within a <see cref="System.Threading.Tasks.Task"/> object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsAsync(Uri databaseUri, DocumentCollection documentCollection, Documents.Client.RequestOptions options = null)
        {
            return TaskHelper.InlineIfPossible(() => this.CreateDocumentCollectionIfNotExistsPrivateAsync(databaseUri, documentCollection, options), null);
        }

        private async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsPrivateAsync(
            Uri databaseUri, DocumentCollection documentCollection, Documents.Client.RequestOptions options)
        {
            if (databaseUri == null)
            {
                throw new ArgumentNullException("databaseUri");
            }

            if (documentCollection == null)
            {
                throw new ArgumentNullException("documentCollection");
            }

            Uri documentCollectionUri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}",
                     databaseUri.OriginalString, Paths.CollectionsPathSegment, Uri.EscapeUriString(documentCollection.Id)), UriKind.Relative);

            try
            {
                return await this.ReadDocumentCollectionAsync(documentCollectionUri, options);
            }
            catch (DocumentClientException dce)
            {
                if (dce.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }
            }

            try
            {
                return await this.CreateDocumentCollectionAsync(databaseUri, documentCollection, options);
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode != HttpStatusCode.Conflict)
                {
                    throw;
                }
            }

            return await this.ReadDocumentCollectionAsync(documentCollectionUri, options);
        }

        /// <summary>
        /// Creates a stored procedure as an asychronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentCollectionUri">the URI of the document collection to create the stored procedure in.</param>
        /// <param name="storedProcedure">the Microsoft.Azure.Documents.StoredProcedure object.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<StoredProcedure>> CreateStoredProcedureAsync(Uri documentCollectionUri, StoredProcedure storedProcedure, Documents.Client.RequestOptions options = null)
        {
            if (documentCollectionUri == null)
            {
                throw new ArgumentNullException("documentCollectionUri");
            }
            return this.CreateStoredProcedureAsync(documentCollectionUri.OriginalString, storedProcedure, options);
        }

        /// <summary>
        /// Creates a trigger as an asychronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentCollectionUri">the URI of the document collection to create the trigger in.</param>
        /// <param name="trigger">the Microsoft.Azure.Documents.Trigger object.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<Trigger>> CreateTriggerAsync(Uri documentCollectionUri, Trigger trigger, Documents.Client.RequestOptions options = null)
        {
            if (documentCollectionUri == null)
            {
                throw new ArgumentNullException("documentCollectionUri");
            }
            return this.CreateTriggerAsync(documentCollectionUri.OriginalString, trigger, options);
        }

        /// <summary>
        /// Creates a user defined function as an asychronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentCollectionUri">the URI of the document collection to create the user defined function in.</param>
        /// <param name="function">the Microsoft.Azure.Documents.UserDefinedFunction object.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(Uri documentCollectionUri, UserDefinedFunction function, Documents.Client.RequestOptions options = null)
        {
            if (documentCollectionUri == null)
            {
                throw new ArgumentNullException("documentCollectionUri");
            }
            return this.CreateUserDefinedFunctionAsync(documentCollectionUri.OriginalString, function, options);
        }

        /// <summary>
        /// Creates a user defined type as an asychronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="databaseUri">the URI of the database to create the user defined type in.</param>
        /// <param name="userDefinedType">the Microsoft.Azure.Documents.UserDefinedType object.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        internal Task<ResourceResponse<UserDefinedType>> CreateUserDefinedTypeAsync(Uri databaseUri, UserDefinedType userDefinedType, Documents.Client.RequestOptions options = null)
        {
            if (databaseUri == null)
            {
                throw new ArgumentNullException("databaseUri");
            }
            return this.CreateUserDefinedTypeAsync(databaseUri.OriginalString, userDefinedType, options);
        }
        #endregion

        #region Upsert operation

        /// <summary>
        /// Upserts a document as an asynchronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentCollectionUri">the URI of the document collection to upsert the document in.</param>
        /// <param name="document">the document object.</param>
        /// <param name="options">The request options for the request.</param>
        /// <param name="disableAutomaticIdGeneration">Disables the automatic id generation, will throw an exception if id is missing.</param>
        /// <param name="cancellationToken">(Optional) <see cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<Document>> UpsertDocumentAsync(Uri documentCollectionUri, object document, Documents.Client.RequestOptions options = null, bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default)
        {
            if (documentCollectionUri == null)
            {
                throw new ArgumentNullException("documentCollectionUri");
            }

            return this.UpsertDocumentAsync(documentCollectionUri.OriginalString, document, options, disableAutomaticIdGeneration, cancellationToken);
        }

        /// <summary>
        /// Upserts a stored procedure as an asynchronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentCollectionUri">the URI of the document collection to upsert the stored procedure in.</param>
        /// <param name="storedProcedure">the Microsoft.Azure.Documents.StoredProcedure object.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedureAsync(Uri documentCollectionUri, StoredProcedure storedProcedure, Documents.Client.RequestOptions options = null)
        {
            if (documentCollectionUri == null)
            {
                throw new ArgumentNullException("documentCollectionUri");
            }
            return this.UpsertStoredProcedureAsync(documentCollectionUri.OriginalString, storedProcedure, options);
        }

        /// <summary>
        /// Upserts a trigger as an asynchronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentCollectionUri">the URI of the document collection to upsert the trigger in.</param>
        /// <param name="trigger">the Microsoft.Azure.Documents.Trigger object.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<Trigger>> UpsertTriggerAsync(Uri documentCollectionUri, Trigger trigger, Documents.Client.RequestOptions options = null)
        {
            if (documentCollectionUri == null)
            {
                throw new ArgumentNullException("documentCollectionUri");
            }
            return this.UpsertTriggerAsync(documentCollectionUri.OriginalString, trigger, options);
        }

        /// <summary>
        /// Upserts a user defined function as an asynchronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentCollectionUri">the URI of the document collection to upsert the user defined function in.</param>
        /// <param name="function">the Microsoft.Azure.Documents.UserDefinedFunction object.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionAsync(Uri documentCollectionUri, UserDefinedFunction function, Documents.Client.RequestOptions options = null)
        {
            if (documentCollectionUri == null)
            {
                throw new ArgumentNullException("documentCollectionUri");
            }
            return this.UpsertUserDefinedFunctionAsync(documentCollectionUri.OriginalString, function, options);
        }

        /// <summary>
        /// Upserts a user defined type as an asynchronous operation  in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="databaseUri">the URI of the database to upsert the user defined type in.</param>
        /// <param name="userDefinedType">the Microsoft.Azure.Documents.UserDefinedType object.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        internal Task<ResourceResponse<UserDefinedType>> UpsertUserDefinedTypeAsync(Uri databaseUri, UserDefinedType userDefinedType, Documents.Client.RequestOptions options = null)
        {
            if (databaseUri == null)
            {
                throw new ArgumentNullException("databaseUri");
            }
            return this.UpsertUserDefinedTypeAsync(databaseUri.OriginalString, userDefinedType, options);
        }
        #endregion

        #region Delete operation
        /// <summary>
        /// Delete a database as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="databaseUri">the URI of the database to delete.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<Documents.Database>> DeleteDatabaseAsync(Uri databaseUri, Documents.Client.RequestOptions options = null)
        {
            if (databaseUri == null)
            {
                throw new ArgumentNullException("databaseUri");
            }
            return this.DeleteDatabaseAsync(databaseUri.OriginalString, options);
        }

        /// <summary>
        /// Delete a document as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentUri">the URI of the document to delete.</param>
        /// <param name="options">The request options for the request.</param>
        /// <param name="cancellationToken">(Optional) <see cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<Document>> DeleteDocumentAsync(Uri documentUri, Documents.Client.RequestOptions options = null, CancellationToken cancellationToken = default)
        {
            if (documentUri == null)
            {
                throw new ArgumentNullException("documentUri");
            }
            return this.DeleteDocumentAsync(documentUri.OriginalString, options, cancellationToken);
        }

        /// <summary>
        /// Delete a collection as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentCollectionUri">the URI of the document collection to delete.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(Uri documentCollectionUri, Documents.Client.RequestOptions options = null)
        {
            if (documentCollectionUri == null)
            {
                throw new ArgumentNullException("documentCollectionUri");
            }
            return this.DeleteDocumentCollectionAsync(documentCollectionUri.OriginalString, options);
        }

        /// <summary>
        /// Delete a stored procedure as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="storedProcedureUri">the URI of the stored procedure to delete.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedureAsync(Uri storedProcedureUri, Documents.Client.RequestOptions options = null)
        {
            if (storedProcedureUri == null)
            {
                throw new ArgumentNullException("storedProcedureUri");
            }
            return this.DeleteStoredProcedureAsync(storedProcedureUri.OriginalString, options);
        }

        /// <summary>
        /// Delete a trigger as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="triggerUri">the URI of the trigger to delete.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<Trigger>> DeleteTriggerAsync(Uri triggerUri, Documents.Client.RequestOptions options = null)
        {
            if (triggerUri == null)
            {
                throw new ArgumentNullException("triggerUri");
            }
            return this.DeleteTriggerAsync(triggerUri.OriginalString, options);
        }

        /// <summary>
        /// Delete a user defined function as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="functionUri">the URI of the user defined function to delete.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(Uri functionUri, Documents.Client.RequestOptions options = null)
        {
            if (functionUri == null)
            {
                throw new ArgumentNullException("functionUri");
            }
            return this.DeleteUserDefinedFunctionAsync(functionUri.OriginalString, options);
        }

        /// <summary>
        /// Delete a conflict as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="conflictUri">the URI of the conflict to delete.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<Conflict>> DeleteConflictAsync(Uri conflictUri, Documents.Client.RequestOptions options = null)
        {
            if (conflictUri == null)
            {
                throw new ArgumentNullException("conflictUri");
            }
            return this.DeleteConflictAsync(conflictUri.OriginalString, options);
        }
        #endregion

        #region Replace operation
        /// <summary>
        /// Replaces a document as an asynchronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentUri">the URI of the document to be updated.</param>
        /// <param name="document">the updated document.</param>
        /// <param name="options">The request options for the request.</param>
        /// <param name="cancellationToken">(Optional) <see cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<Document>> ReplaceDocumentAsync(Uri documentUri, object document, Documents.Client.RequestOptions options = null, CancellationToken cancellationToken = default)
        {
            if (documentUri == null)
            {
                throw new ArgumentNullException("documentUri");
            }
            return this.ReplaceDocumentAsync(documentUri.OriginalString, document, options, cancellationToken);
        }

        /// <summary>
        /// Replaces a document collection as an asynchronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentCollectionUri">the URI of the document collection to be updated.</param>
        /// <param name="documentCollection">the updated document collection.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionAsync(Uri documentCollectionUri, DocumentCollection documentCollection, Documents.Client.RequestOptions options = null)
        {
            if (documentCollectionUri == null)
            {
                throw new ArgumentNullException("documentCollectionUri");
            }

            IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
            return TaskHelper.InlineIfPossible(() => this.ReplaceDocumentCollectionPrivateAsync(
                documentCollection,
                options,
                retryPolicyInstance,
                documentCollectionUri.OriginalString), retryPolicyInstance);
        }

        /// <summary>
        /// Replace the specified stored procedure in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="storedProcedureUri">the URI for the stored procedure to be updated.</param>
        /// <param name="storedProcedure">the updated stored procedure.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureAsync(Uri storedProcedureUri, StoredProcedure storedProcedure, Documents.Client.RequestOptions options = null)
        {
            if (storedProcedureUri == null)
            {
                throw new ArgumentNullException("storedProcedureUri");
            }

            IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
            return TaskHelper.InlineIfPossible(() => this.ReplaceStoredProcedurePrivateAsync(
                storedProcedure,
                options,
                retryPolicyInstance,
                storedProcedureUri.OriginalString), retryPolicyInstance);
        }

        /// <summary>
        /// Replaces a trigger as an asynchronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="triggerUri">the URI for the trigger to be updated.</param>
        /// <param name="trigger">the updated trigger.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<Trigger>> ReplaceTriggerAsync(Uri triggerUri, Trigger trigger, Documents.Client.RequestOptions options = null)
        {
            if (triggerUri == null)
            {
                throw new ArgumentNullException("triggerUri");
            }

            IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
            return TaskHelper.InlineIfPossible(() => this.ReplaceTriggerPrivateAsync(trigger, options, retryPolicyInstance, triggerUri.OriginalString), retryPolicyInstance);
        }

        /// <summary>
        /// Replaces a user defined function as an asynchronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="userDefinedFunctionUri">the URI for the user defined function to be updated.</param>
        /// <param name="function">the updated user defined function.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionAsync(Uri userDefinedFunctionUri, UserDefinedFunction function, Documents.Client.RequestOptions options = null)
        {
            if (userDefinedFunctionUri == null)
            {
                throw new ArgumentNullException("userDefinedFunctionUri");
            }

            IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
            return TaskHelper.InlineIfPossible(() => this.ReplaceUserDefinedFunctionPrivateAsync(function, options, retryPolicyInstance, userDefinedFunctionUri.OriginalString), retryPolicyInstance);
        }

        /// <summary>
        /// Replaces a user defined type as an asynchronous operation in the Azure Cosmos DB service.
        /// </summary>
        /// <param name="userDefinedTypeUri">the URI for the user defined type to be updated.</param>
        /// <param name="userDefinedType">the updated user defined type.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        internal Task<ResourceResponse<UserDefinedType>> ReplaceUserDefinedTypeAsync(Uri userDefinedTypeUri, UserDefinedType userDefinedType, Documents.Client.RequestOptions options = null)
        {
            if (userDefinedTypeUri == null)
            {
                throw new ArgumentNullException("userDefinedTypeUri");
            }

            IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
            return TaskHelper.InlineIfPossible(() => this.ReplaceUserDefinedTypePrivateAsync(userDefinedType, options, retryPolicyInstance, userDefinedTypeUri.OriginalString), retryPolicyInstance);
        }
        #endregion

        #region Read operation
        /// <summary>
        /// Reads a <see cref="Database"/> as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="databaseUri">A URI to the Database resource to be read.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks"/> containing a <see cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/> which wraps a <see cref="Database"/> containing the read resource record.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="databaseUri"/> is not set.</exception>
        /// <exception cref="DocumentClientException">This exception can encapsulate many different types of errors. To determine the specific error always look at the StatusCode property. Some common codes you may get when creating a Document are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>StatusCode</term><description>Reason for exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>404</term><description>NotFound - This means the resource you tried to read did not exist.</description>
        ///     </item>
        ///     <item>
        ///         <term>429</term><description>TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// //Reads a Database resource where 
        /// // - db_id is the ID property of the Database you wish to read. 
        /// var dbLink = UriFactory.CreateDatabaseUri("db_id");
        /// Database database = await client.ReadDatabaseAsync(dbLink);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>
        /// Doing a read of a resource is the most efficient way to get a resource from the service. If you know the resource's ID, do a read instead of a query by ID.
        /// </para>
        /// </remarks>
        /// <seealso cref="Database"/> 
        /// <seealso cref="Microsoft.Azure.Documents.Client.RequestOptions"/>
        /// <seealso cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/>
        /// <seealso cref="System.Threading.Tasks.Task"/>
        public Task<ResourceResponse<Documents.Database>> ReadDatabaseAsync(Uri databaseUri, Documents.Client.RequestOptions options = null)
        {
            if (databaseUri == null)
            {
                throw new ArgumentNullException("databaseUri");
            }
            return this.ReadDatabaseAsync(databaseUri.OriginalString, options);
        }

        /// <summary>
        /// Reads a <see cref="Microsoft.Azure.Documents.Document"/> as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentUri">A URI to the Document resource to be read.</param>
        /// <param name="options">The request options for the request.</param>
        /// <param name="cancellationToken">(Optional) <see cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks"/> containing a <see cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/> which wraps a <see cref="Microsoft.Azure.Documents.Document"/> containing the read resource record.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="documentUri"/> is not set.</exception>
        /// <exception cref="DocumentClientException">This exception can encapsulate many different types of errors. To determine the specific error always look at the StatusCode property. Some common codes you may get when reading a Document are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>StatusCode</term><description>Reason for exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>404</term><description>NotFound - This means the resource you tried to read did not exist.</description>
        ///     </item>
        ///     <item>
        ///         <term>429</term><description>TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// //Reads a Document resource where 
        /// // - db_id is the ID property of the Database
        /// // - coll_id is the ID property of the DocumentCollection
        /// // - doc_id is the ID property of the Document you wish to read. 
        /// var docUri = UriFactory.CreateDocumentUri("db_id", "coll_id", "doc_id");
        /// Document document = await client.ReadDocumentAsync(docUri);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>
        /// Doing a read of a resource is the most efficient way to get a resource from the service. If you know the resource's ID, do a read instead of a query by ID.
        /// </para>
        /// </remarks>
        /// <seealso cref="Microsoft.Azure.Documents.Document"/> 
        /// <seealso cref="Microsoft.Azure.Documents.Client.RequestOptions"/>
        /// <seealso cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/>
        /// <seealso cref="System.Threading.Tasks.Task"/>
        public Task<ResourceResponse<Document>> ReadDocumentAsync(Uri documentUri, Documents.Client.RequestOptions options = null, CancellationToken cancellationToken = default)
        {
            if (documentUri == null)
            {
                throw new ArgumentNullException("documentUri");
            }
            return this.ReadDocumentAsync(documentUri.OriginalString, options, cancellationToken);
        }

        /// <summary>
        /// Reads a <see cref="Microsoft.Azure.Documents.Document"/> as a generic type T from the Azure Cosmos DB service as an asynchronous operation.
        /// </summary>
        /// <param name="documentUri">A URI to the Document resource to be read.</param>
        /// <param name="options">The request options for the request.</param>
        /// <param name="cancellationToken">(Optional) <see cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks"/> containing a <see cref="Microsoft.Azure.Documents.Client.DocumentResponse{T}"/> which wraps a <see cref="Microsoft.Azure.Documents.Document"/> containing the read resource record.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="documentUri"/> is not set.</exception>
        /// <exception cref="DocumentClientException">This exception can encapsulate many different types of errors. To determine the specific error always look at the StatusCode property. Some common codes you may get when reading a Document are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>StatusCode</term><description>Reason for exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>404</term><description>NotFound - This means the resource you tried to read did not exist.</description>
        ///     </item>
        ///     <item>
        ///         <term>429</term><description>TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// //Reads a Document resource where 
        /// // - db_id is the ID property of the Database
        /// // - coll_id is the ID property of the DocumentCollection
        /// // - doc_id is the ID property of the Document you wish to read. 
        /// var docUri = UriFactory.CreateDocumentUri("db_id", "coll_id", "doc_id");
        /// Customer customer = await client.ReadDocumentAsync<Customer>(docUri);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>
        /// Doing a read of a resource is the most efficient way to get a resource from the service. If you know the resource's ID, do a read instead of a query by ID.
        /// </para>
        /// </remarks>
        /// <seealso cref="Microsoft.Azure.Documents.Document"/> 
        /// <seealso cref="Microsoft.Azure.Documents.Client.RequestOptions"/>
        /// <seealso cref="Microsoft.Azure.Documents.Client.DocumentResponse{T}"/>
        /// <seealso cref="System.Threading.Tasks.Task"/>
        public Task<DocumentResponse<T>> ReadDocumentAsync<T>(Uri documentUri, Documents.Client.RequestOptions options = null, CancellationToken cancellationToken = default)
        {
            if (documentUri == null)
            {
                throw new ArgumentNullException("documentUri");
            }
            return this.ReadDocumentAsync<T>(documentUri.OriginalString, options, cancellationToken);
        }

        /// <summary>
        /// Reads a <see cref="DocumentCollection"/> as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="documentCollectionUri">A URI to the DocumentCollection resource to be read.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks"/> containing a <see cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/> which wraps a <see cref="DocumentCollection"/> containing the read resource record.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="documentCollectionUri"/> is not set.</exception>
        /// <exception cref="DocumentClientException">This exception can encapsulate many different types of errors. To determine the specific error always look at the StatusCode property. Some common codes you may get when creating a Document are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>StatusCode</term><description>Reason for exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>404</term><description>NotFound - This means the resource you tried to read did not exist.</description>
        ///     </item>
        ///     <item>
        ///         <term>429</term><description>TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// //Reads a Document resource where 
        /// // - db_id is the ID property of the Database
        /// // - coll_id is the ID property of the DocumentCollection you wish to read. 
        /// var collLink = UriFactory.CreateCollectionUri("db_id", "coll_id");
        /// DocumentCollection coll = await client.ReadDocumentCollectionAsync(collLink);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>
        /// Doing a read of a resource is the most efficient way to get a resource from the service. If you know the resource's ID, do a read instead of a query by ID.
        /// </para>
        /// </remarks>
        /// <seealso cref="DocumentCollection"/> 
        /// <seealso cref="Microsoft.Azure.Documents.Client.RequestOptions"/>
        /// <seealso cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/>
        /// <seealso cref="System.Threading.Tasks.Task"/>
        public Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionAsync(Uri documentCollectionUri, Documents.Client.RequestOptions options = null)
        {
            if (documentCollectionUri == null)
            {
                throw new ArgumentNullException("documentCollectionUri");
            }
            return this.ReadDocumentCollectionAsync(documentCollectionUri.OriginalString, options);
        }

        /// <summary>
        /// Reads a <see cref="StoredProcedure"/> as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="storedProcedureUri">A URI to the StoredProcedure resource to be read.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks"/> containing a <see cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/> which wraps a <see cref="StoredProcedure"/> containing the read resource record.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="storedProcedureUri"/> is not set.</exception>
        /// <exception cref="DocumentClientException">This exception can encapsulate many different types of errors. To determine the specific error always look at the StatusCode property. Some common codes you may get when creating a Document are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>StatusCode</term><description>Reason for exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>404</term><description>NotFound - This means the resource you tried to read did not exist.</description>
        ///     </item>
        ///     <item>
        ///         <term>429</term><description>TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// //Reads a StoredProcedure resource where 
        /// // - db_id is the ID property of the Database
        /// // - coll_id is the ID property of the DocumentCollection 
        /// // - sproc_id is the ID property of the StoredProcedure you wish to read. 
        /// var sprocLink = UriFactory.CreateStoredProcedureUri("db_id", "coll_id", "sproc_id");
        /// StoredProcedure sproc = await client.ReadStoredProcedureAsync(sprocLink);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>
        /// Doing a read of a resource is the most efficient way to get a resource from the service. If you know the resource's ID, do a read instead of a query by ID.
        /// </para>
        /// </remarks>
        /// <seealso cref="StoredProcedure"/> 
        /// <seealso cref="Microsoft.Azure.Documents.Client.RequestOptions"/>
        /// <seealso cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/>
        /// <seealso cref="System.Threading.Tasks.Task"/>
        public Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(Uri storedProcedureUri, Documents.Client.RequestOptions options = null)
        {
            if (storedProcedureUri == null)
            {
                throw new ArgumentNullException("storedProcedureUri");
            }
            return this.ReadStoredProcedureAsync(storedProcedureUri.OriginalString, options);
        }

        /// <summary>
        /// Reads a <see cref="Trigger"/> as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="triggerUri">A URI to the Trigger resource to be read.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks"/> containing a <see cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/> which wraps a <see cref="Trigger"/> containing the read resource record.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="triggerUri"/> is not set.</exception>
        /// <exception cref="DocumentClientException">This exception can encapsulate many different types of errors. To determine the specific error always look at the StatusCode property. Some common codes you may get when creating a Document are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>StatusCode</term><description>Reason for exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>404</term><description>NotFound - This means the resource you tried to read did not exist.</description>
        ///     </item>
        ///     <item>
        ///         <term>429</term><description>TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// //Reads a Trigger resource where 
        /// // - db_id is the ID property of the Database
        /// // - coll_id is the ID property of the DocumentCollection 
        /// // - trigger_id is the ID property of the Trigger you wish to read. 
        /// var triggerLink = UriFactory.CreateTriggerUri("db_id", "coll_id", "trigger_id");
        /// Trigger trigger = await client.ReadTriggerAsync(triggerLink);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>
        /// Doing a read of a resource is the most efficient way to get a resource from the service. If you know the resource's ID, do a read instead of a query by ID.
        /// </para>
        /// </remarks>
        /// <seealso cref="Trigger"/> 
        /// <seealso cref="Microsoft.Azure.Documents.Client.RequestOptions"/>
        /// <seealso cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/>
        /// <seealso cref="System.Threading.Tasks.Task"/>
        public Task<ResourceResponse<Trigger>> ReadTriggerAsync(Uri triggerUri, Documents.Client.RequestOptions options = null)
        {
            if (triggerUri == null)
            {
                throw new ArgumentNullException("triggerUri");
            }
            return this.ReadTriggerAsync(triggerUri.OriginalString, options);
        }

        /// <summary>
        /// Reads a <see cref="UserDefinedFunction"/> as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="functionUri">A URI to the User Defined Function resource to be read.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks"/> containing a <see cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/> which wraps a <see cref="UserDefinedFunction"/> containing the read resource record.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="functionUri"/> is not set.</exception>
        /// <exception cref="DocumentClientException">This exception can encapsulate many different types of errors. To determine the specific error always look at the StatusCode property. Some common codes you may get when creating a Document are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>StatusCode</term><description>Reason for exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>404</term><description>NotFound - This means the resource you tried to read did not exist.</description>
        ///     </item>
        ///     <item>
        ///         <term>429</term><description>TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// //Reads a UserDefinedFunction resource where 
        /// // - db_id is the ID property of the Database
        /// // - coll_id is the ID property of the DocumentCollection 
        /// // - udf_id is the ID property of the UserDefinedFunction you wish to read. 
        /// var udfLink = UriFactory.CreateUserDefinedFunctionUri("db_id", "coll_id", "udf_id");
        /// UserDefinedFunction udf = await client.ReadUserDefinedFunctionAsync(udfLink);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>
        /// Doing a read of a resource is the most efficient way to get a resource from the service. If you know the resource's ID, do a read instead of a query by ID.
        /// </para>
        /// </remarks>
        /// <seealso cref="UserDefinedFunction"/> 
        /// <seealso cref="Microsoft.Azure.Documents.Client.RequestOptions"/>
        /// <seealso cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/>
        /// <seealso cref="System.Threading.Tasks.Task"/>
        public Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionAsync(Uri functionUri, Documents.Client.RequestOptions options = null)
        {
            if (functionUri == null)
            {
                throw new ArgumentNullException("functionUri");
            }
            return this.ReadUserDefinedFunctionAsync(functionUri.OriginalString, options);
        }

        /// <summary>
        /// Reads a <see cref="Microsoft.Azure.Documents.Conflict"/> as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="conflictUri">A URI to the Conflict resource to be read.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks"/> containing a <see cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/> which wraps a <see cref="Microsoft.Azure.Documents.Conflict"/> containing the read resource record.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="conflictUri"/> is not set.</exception>
        /// <exception cref="DocumentClientException">This exception can encapsulate many different types of errors. To determine the specific error always look at the StatusCode property. Some common codes you may get when creating a Document are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>StatusCode</term><description>Reason for exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>404</term><description>NotFound - This means the resource you tried to read did not exist.</description>
        ///     </item>
        ///     <item>
        ///         <term>429</term><description>TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// //Reads a Conflict resource where 
        /// // - db_id is the ID property of the Database
        /// // - coll_id is the ID property of the DocumentCollection
        /// // - conflict_id is the ID property of the Conflict you wish to read. 
        /// var conflictLink = UriFactory.CreateConflictUri("db_id", "coll_id", "conflict_id");
        /// Conflict conflict = await client.ReadConflictAsync(conflictLink);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>
        /// Doing a read of a resource is the most efficient way to get a resource from the service. If you know the resource's ID, do a read instead of a query by ID.
        /// </para>
        /// </remarks>
        /// <seealso cref="Microsoft.Azure.Documents.Conflict"/> 
        /// <seealso cref="Microsoft.Azure.Documents.Client.RequestOptions"/>
        /// <seealso cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/>
        /// <seealso cref="System.Threading.Tasks.Task"/>
        public Task<ResourceResponse<Conflict>> ReadConflictAsync(Uri conflictUri, Documents.Client.RequestOptions options = null)
        {
            if (conflictUri == null)
            {
                throw new ArgumentNullException("conflictUri");
            }
            return this.ReadConflictAsync(conflictUri.OriginalString, options);
        }

        /// <summary>
        /// Reads a <see cref="Microsoft.Azure.Documents.Schema"/> as an asynchronous operation.
        /// </summary>
        /// <param name="schemaUri">A URI to the Schema resource to be read.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks"/> containing a <see cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/> which wraps a <see cref="Microsoft.Azure.Documents.Schema"/> containing the read resource record.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="schemaUri"/> is not set.</exception>
        /// <exception cref="DocumentClientException">This exception can encapsulate many different types of errors. To determine the specific error always look at the StatusCode property. Some common codes you may get when reading a Schema are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>StatusCode</term><description>Reason for exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>404</term><description>NotFound - This means the resource you tried to read did not exist.</description>
        ///     </item>
        ///     <item>
        ///         <term>429</term><description>TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// //Reads a Document resource where 
        /// // - db_id is the ID property of the Database
        /// // - coll_id is the ID property of the DocumentCollection
        /// // - schema_id is the ID property of the Document you wish to read. 
        /// var docLink = UriFactory.CreateDocumentUri("db_id", "coll_id", "schema_id");
        /// Schema schema = await client.ReadSchemaAsync(schemaLink);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>
        /// Doing a read of a resource is the most efficient way to get a resource from the service. If you know the resource's ID, do a read instead of a query by ID.
        /// </para>
        /// </remarks>
        /// <seealso cref="Microsoft.Azure.Documents.Schema"/> 
        /// <seealso cref="Microsoft.Azure.Documents.Client.RequestOptions"/>
        /// <seealso cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/>
        /// <seealso cref="System.Threading.Tasks.Task"/>
        internal Task<ResourceResponse<Schema>> ReadSchemaAsync(Uri schemaUri, Documents.Client.RequestOptions options = null)
        {
            if (schemaUri == null)
            {
                throw new ArgumentNullException("schemaUri");
            }
            return this.ReadSchemaAsync(schemaUri.OriginalString, options);
        }

        /// <summary>
        /// Reads a <see cref="UserDefinedType"/> as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="userDefinedTypeUri">A URI to the UserDefinedType resource to be read.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks"/> containing a <see cref="ResourceResponse{T}"/> which wraps a <see cref="UserDefinedType"/> containing the read resource record.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="userDefinedTypeUri"/> is not set.</exception>
        /// <exception cref="DocumentClientException">This exception can encapsulate many different types of errors. To determine the specific error always look at the StatusCode property. Some common codes you may get when creating a Document are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>StatusCode</term><description>Reason for exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>404</term><description>NotFound - This means the resource you tried to read did not exist.</description>
        ///     </item>
        ///     <item>
        ///         <term>429</term><description>TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// //Reads a UserDefinedType resource where 
        /// // - db_id is the ID property of the Database
        /// // - userDefinedType_id is the ID property of the UserDefinedType you wish to read. 
        /// var userDefinedTypeLink = UriFactory.CreateUserDefinedTypeUri("db_id", "userDefinedType_id");
        /// UserDefinedType userDefinedType = await client.ReadUserDefinedTypeAsync(userDefinedTypeLink);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>
        /// Doing a read of a resource is the most efficient way to get a resource from the service. If you know the resource's ID, do a read instead of a query by ID.
        /// </para>
        /// </remarks>
        /// <seealso cref="UserDefinedType"/> 
        /// <seealso cref="RequestOptions"/>
        /// <seealso cref="ResourceResponse{T}"/>
        /// <seealso cref="Task"/>
        internal Task<ResourceResponse<UserDefinedType>> ReadUserDefinedTypeAsync(Uri userDefinedTypeUri, Documents.Client.RequestOptions options = null)
        {
            if (userDefinedTypeUri == null)
            {
                throw new ArgumentNullException("userDefinedTypeUri");
            }
            return this.ReadUserDefinedTypeAsync(userDefinedTypeUri.OriginalString, options);
        }

        /// <summary>
        /// Reads a <see cref="Microsoft.Azure.Documents.Snapshot"/> as an asynchronous operation from the Azure Cosmos DB service.
        /// </summary>
        /// <param name="snapshotUri">A URI to the Snapshot resource to be read.</param>
        /// <param name="options">The request options for the request.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks"/> containing a <see cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/> which wraps a <see cref="Microsoft.Azure.Documents.Snapshot"/> containing the read resource record.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="snapshotUri"/> is not set.</exception>
        /// <exception cref="DocumentClientException">This exception can encapsulate many different types of errors. To determine the specific error always look at the StatusCode property. Some common codes you may get when reading a Snapshot are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>StatusCode</term><description>Reason for exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>404</term><description>NotFound - This means the resource you tried to read did not exist.</description>
        ///     </item>
        ///     <item>
        ///         <term>429</term><description>TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// //Reads a Snapshot resource where 
        /// // - snapshot_id is the ID property of the Snapshot you wish to read. 
        /// var snapshotLink = UriFactory.CreateSnapshotUri("snapshot_id");
        /// Snapshot snapshot = await client.ReadSnapshotAsync(snapshotLink);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>
        /// Doing a read of a resource is the most efficient way to get a resource from the service. If you know the resource's ID, do a read instead of a query by ID.
        /// </para>
        /// </remarks>
        /// <seealso cref="Microsoft.Azure.Documents.Snapshot"/> 
        /// <seealso cref="Microsoft.Azure.Documents.Client.RequestOptions"/>
        /// <seealso cref="Microsoft.Azure.Documents.Client.ResourceResponse{T}"/>
        /// <seealso cref="System.Threading.Tasks.Task"/>
        internal Task<ResourceResponse<Snapshot>> ReadSnapshotAsync(Uri snapshotUri, Documents.Client.RequestOptions options = null)
        {
            if (snapshotUri == null)
            {
                throw new ArgumentNullException("snapshotUri");
            }

            return this.ReadSnapshotAsync(snapshotUri, options);
        }

        #endregion
    }
}
