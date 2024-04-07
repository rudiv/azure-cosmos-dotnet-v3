//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a conflict in the Azure Cosmos DB service.
    /// </summary>
    /// <remarks>
    /// On rare occasions, during an async operation (insert, replace and delete), a version conflict may occur on a resource during fail over or multi master scenarios.
    /// The conflicting resource is persisted as a Conflict resource.  
    /// Inspecting Conflict resources will allow you to determine which operations and resources resulted in conflicts.
    /// This is not related to operations returning a Conflict status code.
    /// </remarks>
    public class ConflictProperties
    {
        /// <summary>
        /// Gets the Id of the resource in the Azure Cosmos DB service.
        /// </summary>
        /// <value>The Id associated with the resource.</value>
        /// <remarks>
        /// <para>
        /// Every resource within an Azure Cosmos DB database account needs to have a unique identifier. 
        /// </para>
        /// <para>
        /// The following characters are restricted and cannot be used in the Id property:
        ///  '/', '\\', '?', '#'
        /// </para>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName(name: Documents.Constants.Properties.Id)]
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the operation that resulted in the conflict in the Azure Cosmos DB service.
        /// </summary>
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [System.Text.Json.Serialization.JsonPropertyName(name: Documents.Constants.Properties.OperationType)]
        public OperationKind OperationKind { get; internal set; }

        /// <summary>
        /// Gets the self-link associated with the resource from the Azure Cosmos DB service.
        /// </summary>
        /// <value>The self-link associated with the resource.</value> 
        /// <remarks>
        /// A self-link is a static addressable Uri for each resource within a database account and follows the Azure Cosmos DB resource model.
        /// E.g. a self-link for a document could be dbs/db_resourceid/colls/coll_resourceid/documents/doc_resourceid
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName(name: Documents.Constants.Properties.SelfLink)]
[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public string SelfLink { get; private set; }

        [Newtonsoft.Json.JsonConverter(typeof(ConflictResourceTypeJsonConverter))]
        [System.Text.Json.Serialization.JsonPropertyName(name: Documents.Constants.Properties.ResourceType)]
        internal Type ResourceType { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName(name: Documents.Constants.Properties.SourceResourceId)]
        internal string SourceResourceId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName(name: Documents.Constants.Properties.Content)]
        internal string Content { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName(name: Documents.Constants.Properties.ConflictLSN)]
        internal long ConflictLSN { get; set; }

        /// <summary>
        /// This contains additional values for scenarios where the SDK is not aware of new fields. 
        /// This ensures that if resource is read and updated none of the fields will be lost in the process.
        /// </summary>
        [Newtonsoft.Json.JsonExtensionData]
        internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
    }
}
