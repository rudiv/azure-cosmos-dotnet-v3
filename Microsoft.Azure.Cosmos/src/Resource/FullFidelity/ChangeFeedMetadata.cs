//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Text.Json.Serialization;
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The metadata of a change feed resource with <see cref="ChangeFeedMode"/> is initialized to <see cref="ChangeFeedMode.AllVersionsAndDeletes"/>.
    /// </summary>
#if PREVIEW
    public
#else
    internal
#endif 
        class ChangeFeedMetadata
    {
        /// <summary>
        /// New instance of meta data for <see cref="ChangeFeedItem{T}"/> created.
        /// </summary>
        /// <param name="conflictResolutionTimestamp"></param>
        /// <param name="lsn"></param>
        /// <param name="operationType"></param>
        /// <param name="previousLsn"></param>
        public ChangeFeedMetadata(
            DateTime conflictResolutionTimestamp,
            long lsn,
            ChangeFeedOperationType operationType,
            long previousLsn)
        {
            this.ConflictResolutionTimestamp = conflictResolutionTimestamp;
            this.Lsn = lsn;
            this.OperationType = operationType;
            this.PreviousLsn = previousLsn;
        }

        /// <summary>
        /// The conflict resolution timestamp.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: "crts")]
[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        [Newtonsoft.Json.JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime ConflictResolutionTimestamp { get; }

        /// <summary>
        /// The current logical sequence number.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: "lsn")]
[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public long Lsn { get; }

        /// <summary>
        /// The change feed operation type.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: "operationType")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
        public ChangeFeedOperationType OperationType { get; }

        /// <summary>
        /// The previous logical sequence number.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: "previousImageLSN")]
[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public long PreviousLsn { get; }

        /// <summary>
        /// Used to distinquish explicit deletes (e.g. via DeleteItem) from deletes caused by TTL expiration (a collection may define time-to-live policy for documents).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName(name: "timeToLiveExpired")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsTimeToLiveExpired { get; }
    }
}
