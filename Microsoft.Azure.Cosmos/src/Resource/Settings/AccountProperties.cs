//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Microsoft.Azure.Cosmos.Telemetry;
    using Microsoft.Azure.Documents;
    using STJ;

    /// <summary> 
    /// Represents a <see cref="AccountProperties"/>. A AccountProperties is the container for databases in the Azure Cosmos DB service.
    /// </summary>
    public class AccountProperties
    {
        private Collection<AccountRegion> readRegions;
        private Collection<AccountRegion> writeRegions;

        internal readonly Lazy<IDictionary<string, object>> QueryEngineConfigurationInternal;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountProperties"/> class.
        /// </summary>
        public AccountProperties()
        {
            this.QueryEngineConfigurationInternal = new Lazy<IDictionary<string, object>>(() => this.QueryStringToDictConverter());
        }

        /// <summary>
        /// Gets the list of locations representing the writable regions of
        /// this database account from the Azure Cosmos DB service.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<AccountRegion> WritableRegions => this.WriteLocationsInternal;

        /// <summary>
        /// Gets the list of locations representing the readable regions of
        /// this database account from the Azure Cosmos DB service.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<AccountRegion> ReadableRegions => this.ReadLocationsInternal;

        /// <summary>
        /// Gets the Id of the resource in the Azure Cosmos DB service.
        /// </summary>
        /// <value>The Id associated with the resource.</value>
        /// <remarks>
        /// <para>
        /// Every resource within an Azure Cosmos DB database account needs to have a unique identifier. 
        /// Unlike <see cref="Documents.Resource.ResourceId"/>, which is set internally, this Id is settable by the user and is not immutable.
        /// </para>
        /// <para>
        /// The following characters are restricted and cannot be used in the Id property:
        ///  '/', '\\', '?', '#'
        /// </para>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.Id)]
        public string Id
        {
            get;
            set;
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
        [JsonInclude]
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the Resource Id associated with the resource in the Azure Cosmos DB service.
        /// </summary>
        /// <value>
        /// The Resource Id associated with the resource.
        /// </value>
        /// <remarks>
        /// A Resource Id is the unique, immutable, identifier assigned to each Azure Cosmos DB 
        /// resource whether that is a database, a collection or a document.
        /// These resource ids are used when building up SelfLinks, a static addressable Uri for each resource within a database account.
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.RId)]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        [JsonInclude]
        internal string ResourceId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.WritableLocations)]
        [JsonInclude]
        internal Collection<AccountRegion> WriteLocationsInternal
        {
            get
            {
                if (this.writeRegions == null)
                {
                    this.writeRegions = new Collection<AccountRegion>();
                }
                return this.writeRegions;
            }
            set => this.writeRegions = value;
        }

        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.ReadableLocations)]
        [JsonInclude]
        internal Collection<AccountRegion> ReadLocationsInternal
        {
            get
            {
                if (this.readRegions == null)
                {
                    this.readRegions = new Collection<AccountRegion>();
                }
                return this.readRegions;
            }
            set => this.readRegions = value;
        }

        /// <summary>
        /// Gets the storage quota for media storage in the databaseAccount from the Azure Cosmos DB service.
        /// </summary>
        /// <value>
        /// The storage quota in measured MBs.
        /// </value>
        /// <remarks>
        /// This value is retrieved from the gateway.
        /// </remarks>
        internal long MaxMediaStorageUsageInMB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current attachment content (media) usage in MBs from the Azure Cosmos DB service.
        /// </summary>
        /// <value>
        /// The attachment content (media) usage in MBs.
        /// </value>
        /// <remarks>
        /// The value is retrieved from the gateway. The value is returned from cached information updated periodically 
        /// and is not guaranteed to be real time.
        /// </remarks>
        internal long MediaStorageUsageInMB
        {
            get;
            set;
        }

        /// <summary>
        /// The cumulative sum of current sizes of created collection in MB
        /// Value is returned from cached information which is updated periodically and is not guaranteed to be real time
        /// TODO remove this property tfs 4442779
        /// </summary>
        internal long ConsumedDocumentStorageInMB
        {
            get;
            set;
        }

        /// <summary>
        /// The cumulative sum of maximum sizes of created collection in MB
        /// Value is returned from cached information which is updated periodically and is not guaranteed to be real time
        /// TODO remove this property tfs 4442779
        /// </summary>
        internal long ReservedDocumentStorageInMB
        {
            get;
            set;
        }

        /// <summary>
        /// The provisioned documented storage capacity for the database account
        /// Value is returned from cached information which is updated periodically and is not guaranteed to be real time
        /// TODO remove this property tfs 4442779
        /// </summary>
        internal long ProvisionedDocumentStorageInMB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="Consistency"/> from the Azure Cosmos DB service.
        /// </summary>
        /// <value>
        /// The ConsistencySetting.
        /// </value>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.UserConsistencyPolicy)]
        public AccountConsistency Consistency { get; set; }

        /// <summary>
        /// Gets the self-link for Address Routing Table in the databaseAccount
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.AddressesLink)]
        [JsonInclude]
        internal string AddressesLink { get; set; }

        /// <summary>
        /// Gets the ReplicationPolicy properties
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.UserReplicationPolicy)]
        [JsonInclude]
        internal ReplicationPolicy ReplicationPolicy { get; set; }

        /// <summary>
        /// Gets the SystemReplicationPolicy 
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.SystemReplicationPolicy)]
        [JsonInclude]
        internal ReplicationPolicy SystemReplicationPolicy { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.ReadPolicy)]
        [JsonInclude]
        internal ReadPolicy ReadPolicy { get; set; }

        internal IDictionary<string, object> QueryEngineConfiguration => this.QueryEngineConfigurationInternal.Value;

        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.QueryEngineConfiguration)]
        [JsonInclude]
        internal string QueryEngineConfigurationString { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.EnableMultipleWriteLocations)]
        [JsonInclude]
        internal bool EnableMultipleWriteLocations { get; set; }

        private IDictionary<string, object> QueryStringToDictConverter()
        {
            if (!string.IsNullOrEmpty(this.QueryEngineConfigurationString))
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(this.QueryEngineConfigurationString, InternalDocumentContext.Default.DictionaryStringObject);
            }
            else
            {
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// This contains additional values for scenarios where the SDK is not aware of new fields. 
        /// This ensures that if resource is read and updated none of the fields will be lost in the process.
        /// </summary>
        [JsonExtensionData]
        internal IDictionary<string, JsonElement> AdditionalProperties { get; private set; }
    }
}
