//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Query.Core
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    public sealed class SqlQueryResumeFilter
    {
        [JsonPropertyName("value")]
        public IReadOnlyList<SqlQueryResumeValue> ResumeValues { get; }

        [JsonPropertyName("rid")]
        public string Rid { get; }

        [JsonPropertyName("exclude")]
        public bool Exclude { get; }

        public SqlQueryResumeFilter(
            IReadOnlyList<SqlQueryResumeValue> resumeValues,
            string rid,
            bool exclude)
        {
            if ((resumeValues == null) || (resumeValues.Count == 0))
            {
                throw new ArgumentException($"{nameof(resumeValues)} can not be empty.");
            }

            this.ResumeValues = resumeValues;
            this.Rid = rid;
            this.Exclude = exclude;
        }
    }
}
