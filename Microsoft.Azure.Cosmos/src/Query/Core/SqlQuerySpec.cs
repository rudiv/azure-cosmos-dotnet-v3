//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Query.Core;

namespace Microsoft.Azure.Cosmos.Query.Core
{
    using System;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a SQL query in the Azure Cosmos DB service.
    /// </summary>
    [JsonConverter(typeof(IgnoreEmptyParametersConverter))]
    public sealed class SqlQuerySpec
    {
        private SqlParameterCollection parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.Documents.SqlQuerySpec"/> class for the Azure Cosmos DB service.</summary>
        /// <remarks> 
        /// The default constructor initializes any fields to their default values.
        /// </remarks>
        public SqlQuerySpec()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.Documents.SqlQuerySpec"/> class for the Azure Cosmos DB service.
        /// </summary>
        /// <param name="queryText">The text of the query.</param>
        public SqlQuerySpec(string queryText)
            : this(queryText, new SqlParameterCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.Documents.SqlQuerySpec"/> class for the Azure Cosmos DB service.
        /// </summary>
        /// <param name="queryText">The text of the database query.</param>
        /// <param name="parameters">The <see cref="T:Microsoft.Azure.Documents.SqlParameterCollection"/> instance, which represents the collection of query parameters.</param>
        public SqlQuerySpec(string queryText, SqlParameterCollection parameters)
            : this(queryText, parameters, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.Documents.SqlQuerySpec"/> class for the Azure Cosmos DB service.
        /// </summary>
        /// <param name="queryText">The text of the database query.</param>
        /// <param name="parameters">The <see cref="T:Microsoft.Azure.Documents.SqlParameterCollection"/> instance, which represents the collection of query parameters.</param>
        /// <param name="resumeFilter">The <see cref="T:Microsoft.Azure.Cosmos.Query.Core.SqlQueryResumeFilter"/> instance, which represents the query resume filter.</param>
        public SqlQuerySpec(string queryText, SqlParameterCollection parameters, SqlQueryResumeFilter resumeFilter)
        {
            this.QueryText = queryText;
            this.parameters = parameters ?? throw new ArgumentNullException("parameters");
            this.ResumeFilter = resumeFilter;
        }

        /// <summary>
        /// Gets or sets the text of the Azure Cosmos DB database query.
        /// </summary>
        /// <value>The text of the database query.</value>
        [JsonPropertyName("query")]
        public string QueryText { get; set; }

        /// <summary>
        /// Gets or sets the ClientQL Compatibility Level supported by the client.
        /// </summary>
        /// <value>The integer value representing the compatibility of the client.</value>
        [JsonPropertyName("clientQLCompatibilityLevel")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? ClientQLCompatibilityLevel { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:Microsoft.Azure.Documents.SqlParameterCollection"/> instance, which represents the collection of Azure Cosmos DB query parameters.
        /// </summary>
        /// <value>The <see cref="T:Microsoft.Azure.Documents.SqlParameterCollection"/> instance.</value>
        [JsonPropertyName("parameters")]
        public SqlParameterCollection Parameters
        {
            get => this.parameters;
            set => this.parameters = value ?? throw new ArgumentNullException("value");
        }

        [JsonPropertyName("resumeFilter")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public SqlQueryResumeFilter ResumeFilter { get; set; }
    }
}

public class IgnoreEmptyParametersConverter : JsonConverter<SqlQuerySpec>
{
    public override SqlQuerySpec Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<SqlQuerySpec>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, SqlQuerySpec value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("query", value.QueryText);
        if (value.ClientQLCompatibilityLevel.HasValue && value.ClientQLCompatibilityLevel.Value != default)
        {
            writer.WriteNumber("clientQLCompatibilityLevel", value.ClientQLCompatibilityLevel.Value);
        }

        if (value.Parameters != null && value.Parameters.Count > 0)
        {
            writer.WritePropertyName("parameters");
            JsonSerializer.Serialize(writer, value.Parameters, options);
        }
        
        if (value.ResumeFilter != null)
        {
            writer.WritePropertyName("resumeFilter");
            JsonSerializer.Serialize(writer, value.ResumeFilter, options);
        }

        writer.WriteEndObject();
    }
}
