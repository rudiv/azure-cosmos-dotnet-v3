//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Telemetry.Models
{
    using System;
    using Newtonsoft.Json;
    using Util;

    [Serializable]
    internal sealed class Compute
    {
        [JsonConstructor]
        public Compute(
            string vMId,
            string location,
            string sKU,
            string azEnvironment,
            string oSType,
            string vMSize)
        {
            this.Location = location;
            this.SKU = sKU;
            this.AzEnvironment = azEnvironment;
            this.OSType = oSType;
            this.VMSize = vMSize;
            this.VMId = $"{VmMetadataApiHandler.VmIdPrefix}{vMId}";
        }

        [System.Text.Json.Serialization.JsonPropertyName(name: "location")]
        internal string Location { get; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "sku")]
        internal string SKU { get; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "azEnvironment")]
        internal string AzEnvironment { get; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "osType")]
        internal string OSType { get; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "vmSize")]
        internal string VMSize { get; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "vmId")]
        internal string VMId { get; }
    }

}
