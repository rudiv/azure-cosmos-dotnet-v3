﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Telemetry.Models
{
    using System;
    using System.Collections.Generic;
    using HdrHistogram;
    using Microsoft.Azure.Cosmos.Telemetry;
    using Microsoft.Azure.Cosmos.Util;
    using Newtonsoft.Json;

    [Serializable]
    internal sealed class MetricInfo
    {
        internal MetricInfo(string metricsName, string unitName)
        {
            this.MetricsName = metricsName;
            this.UnitName = unitName;
        }

        public MetricInfo(string metricsName,
            string unitName,
            double mean = 0,
            long count = 0,
            long min = 0,
            long max = 0,
            IReadOnlyDictionary<double, double> percentiles = null)
            : this(metricsName, unitName)
        {
            this.Mean = mean;
            this.Count = count;
            this.Min = min;
            this.Max = max;
            this.Percentiles = percentiles;
        }

        [System.Text.Json.Serialization.JsonPropertyName(name: "metricsName")]
        internal string MetricsName { get; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "unitName")]
        internal string UnitName { get; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "mean")]
        internal double Mean { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "count")]
        internal long Count { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "min")]
        internal double Min { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "max")]
        internal double Max { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName(name: "percentiles")]
        internal IReadOnlyDictionary<double, double> Percentiles { get; set; }

        /// <summary>
        /// It will set the current object with the aggregated values from the given histogram
        /// </summary>
        /// <param name="histogram"></param>
        /// <param name="adjustment"></param>
        /// <returns>MetricInfo</returns>
        internal MetricInfo SetAggregators(LongConcurrentHistogram histogram, double adjustment = 1)
        {
            if (histogram != null)
            {
                this.Count = histogram.TotalCount;
                this.Max = histogram.GetMaxValue() / adjustment;
                this.Min = histogram.GetMinValue() / adjustment;
                this.Mean = histogram.GetMean() / adjustment;
                IReadOnlyDictionary<double, double> percentile = new Dictionary<double, double>
                {
                    { ClientTelemetryOptions.Percentile50,  histogram.GetValueAtPercentile(ClientTelemetryOptions.Percentile50) / adjustment },
                    { ClientTelemetryOptions.Percentile90,  histogram.GetValueAtPercentile(ClientTelemetryOptions.Percentile90) / adjustment },
                    { ClientTelemetryOptions.Percentile95,  histogram.GetValueAtPercentile(ClientTelemetryOptions.Percentile95) / adjustment },
                    { ClientTelemetryOptions.Percentile99,  histogram.GetValueAtPercentile(ClientTelemetryOptions.Percentile99) / adjustment },
                    { ClientTelemetryOptions.Percentile999, histogram.GetValueAtPercentile(ClientTelemetryOptions.Percentile999) / adjustment }
                };
                this.Percentiles = percentile;
            }
            return this;
        }
    }
}
