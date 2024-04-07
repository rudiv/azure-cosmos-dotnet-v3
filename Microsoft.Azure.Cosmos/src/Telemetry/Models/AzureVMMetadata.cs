//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Telemetry.Models
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    internal sealed class AzureVMMetadata
    {
        public AzureVMMetadata(Compute compute)
        {
            this.Compute = compute;
        }

        [System.Text.Json.Serialization.JsonPropertyName(name: "compute")]
        internal Compute Compute { get; }
    }
}
