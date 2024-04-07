//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text.Json.Serialization;
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Spatial index specification
    /// </summary>
    /// <example>
    /// <![CDATA[
    ///     "spatialIndexes":
    ///     [
    ///         {  
    ///             "path":"/'region'/?",
    ///             "types":["Polygon"],
    ///             "boundingBox": 
    ///                 {
    ///                    "xmin":0, 
    ///                    "ymin":0,
    ///                    "xmax":10, 
    ///                    "ymax":10
    ///                 }
    ///        }
    ///   ]
    /// ]]>
    /// </example>
    public sealed class SpatialPath
    {
        private Collection<SpatialType> spatialTypesInternal;

        /// <summary>
        /// Path in JSON document to index
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.Path)]
        public string Path { get; set; }

        /// <summary>
        /// Path's spatial type
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: Constants.Properties.Types)]
        [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
        public Collection<SpatialType> SpatialTypes
        {
            get
            {
                if (this.spatialTypesInternal == null)
                {
                    this.spatialTypesInternal = new Collection<SpatialType>();
                }
                return this.spatialTypesInternal;
            }
            internal set => this.spatialTypesInternal = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Gets or sets the bounding box
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: "boundingBox")]
[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public BoundingBoxProperties BoundingBox
        {
            get; set;
        }

        /// <summary>
        /// This contains additional values for scenarios where the SDK is not aware of new fields. 
        /// This ensures that if resource is read and updated none of the fields will be lost in the process.
        /// </summary>
        [Newtonsoft.Json.JsonExtensionData]
        internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
    }
}
