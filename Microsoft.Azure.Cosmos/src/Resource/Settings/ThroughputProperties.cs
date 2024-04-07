// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a throughput of the resources in the Azure Cosmos DB service.
    /// It is the standard pricing for the resource in the Azure Cosmos DB service.
    /// </summary>
    /// <remarks>
    /// It contains provisioned container throughput in measurement of request units per second in the Azure Cosmos service.
    /// </remarks>
    /// <seealso href="https://docs.microsoft.com/azure/cosmos-db/request-units">Request Units</seealso>
    /// <seealso href="https://docs.microsoft.com/azure/cosmos-db/set-throughput">Set throughput on resources</seealso>
    /// <example>
    /// The example below fetch the ThroughputProperties on testContainer.
    /// <code language="c#">
    /// <![CDATA[ 
    /// ThroughputProperties throughputProperties = await testContainer.ReadThroughputAsync().Resource;
    /// ]]>
    /// </code>
    /// </example>
    public class ThroughputProperties
    {
        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        [Newtonsoft.Json.JsonConstructor]
        internal ThroughputProperties()
        {
        }

        /// <summary>
        /// Create a instance for fixed throughput
        /// </summary>
        internal ThroughputProperties(OfferContentProperties offerContentProperties)
        {
            this.OfferVersion = Constants.Offers.OfferVersion_V2;
            this.Content = offerContentProperties;
        }

        /// <summary>
        /// Gets the entity tag associated with the resource from the Azure Cosmos DB service.
        /// </summary>
        /// <value>
        /// The entity tag associated with the resource.
        /// </value>
        /// <remarks>
        /// ETags are used for concurrency checking when updating resources.
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.ETag)]
[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public string ETag { get; private set; }

        /// <summary>
        /// Gets the last modified time stamp associated with <see cref="DatabaseProperties" /> from the Azure Cosmos DB service.
        /// </summary>
        /// <value>The last modified time stamp associated with the resource.</value>
        [Newtonsoft.Json.JsonConverter(typeof(UnixDateTimeConverter))]
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.LastModified)]
[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? LastModified { get; private set; }

        /// <summary>
        /// Gets the provisioned throughput for a resource in measurement of request units per second in the Azure Cosmos service.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public int? Throughput
        {
            get => this.Content?.OfferThroughput;
            private set => this.Content = OfferContentProperties.CreateManualOfferConent(value.Value);
        }

        /// <summary>
        /// The maximum throughput the autoscale will scale to.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public int? AutoscaleMaxThroughput => this.Content?.OfferAutoscaleSettings?.MaxThroughput;

        /// <summary>
        /// The amount to increment if the maximum RUs is getting throttled.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        internal int? AutoUpgradeMaxThroughputIncrementPercentage => this.Content?.OfferAutoscaleSettings?.AutoscaleAutoUpgradeProperties?.ThroughputProperties?.IncrementPercent;

        /// <summary>
        /// The Throughput properties for manual provisioned throughput offering
        /// </summary>
        /// <param name="throughput">The current provisioned throughput for the resource.</param>
        /// <returns>Returns a ThroughputProperties for manual throughput</returns>
        public static ThroughputProperties CreateManualThroughput(int throughput)
        {
            if (throughput <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(throughput)} must be greater than 0");
            }

            return new ThroughputProperties(OfferContentProperties.CreateManualOfferConent(throughput));
        }

        /// <summary>
        /// The Throughput properties for autoscale provisioned throughput offering
        /// </summary>
        /// <param name="autoscaleMaxThroughput">The maximum throughput the resource can scale to.</param>
        /// <returns>Returns a ThroughputProperties for autoscale provisioned throughput</returns>
        public static ThroughputProperties CreateAutoscaleThroughput(
            int autoscaleMaxThroughput)
        {
            if (autoscaleMaxThroughput <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(autoscaleMaxThroughput)} must be greater than 0");
            }

            return new ThroughputProperties(OfferContentProperties.CreateAutoscaleOfferConent(
                startingMaxThroughput: autoscaleMaxThroughput,
                autoUpgradeMaxThroughputIncrementPercentage: null));
        }

        internal static ThroughputProperties CreateManualThroughput(int? throughput)
        {
            if (!throughput.HasValue)
            {
                return null;
            }

            return CreateManualThroughput(throughput.Value);
        }

        internal static ThroughputProperties CreateAutoscaleThroughput(
            int maxAutoscaleThroughput,
            int? autoUpgradeMaxThroughputIncrementPercentage = null)
        {
            return new ThroughputProperties(OfferContentProperties.CreateAutoscaleOfferConent(
                maxAutoscaleThroughput,
                autoUpgradeMaxThroughputIncrementPercentage));
        }

        /// <summary>
        /// Gets the self-link associated with the resource from the Azure Cosmos DB service.
        /// </summary>
        /// <value>The self-link associated with the resource.</value> 
        /// <remarks>
        /// A self-link is a static addressable Uri for each resource within a database account and follows the Azure Cosmos DB resource model.
        /// E.g. a self-link for a document could be dbs/db_resourceid/colls/coll_resourceid/documents/doc_resourceid
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.SelfLink)]
[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public string SelfLink { get; private set; }

        /// <summary>
        /// Gets the offer rid.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.RId)]
[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        internal string OfferRID { get; private set; }

        /// <summary>
        /// Gets the resource rid.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.OfferResourceId)]
[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        internal string ResourceRID { get; private set; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "content")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        internal OfferContentProperties Content { get; set; }

        /// <summary>
        /// Gets the version of this offer resource in the Azure Cosmos DB service.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.OfferVersion)]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        internal string OfferVersion { get; private set; }

        /// <summary>
        /// This contains additional values for scenarios where the SDK is not aware of new fields. 
        /// This ensures that if resource is read and updated none of the fields will be lost in the process.
        /// </summary>
        [Newtonsoft.Json.JsonExtensionData]
        internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
    }
}
