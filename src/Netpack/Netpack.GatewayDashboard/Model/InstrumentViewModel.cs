// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Otlp.Model;
using Netpack.GatewayDashboard.Otlp.Model.MetricValues;

namespace Netpack.GatewayDashboard.Model;

public class InstrumentViewModel
{
    public OtlpInstrumentSummary? Instrument { get; private set; }
    public List<DimensionScope>? MatchedDimensions { get; private set; }

    public List<Func<Task>> DataUpdateSubscriptions { get; } = [];
    public string? Theme { get; set; }
    public bool ShowCount { get; set; }

    public async Task UpdateDataAsync(OtlpInstrumentSummary instrument, List<DimensionScope> matchedDimensions)
    {
        Instrument = instrument;
        MatchedDimensions = matchedDimensions;

        foreach (var subscription in DataUpdateSubscriptions)
        {
            await subscription().ConfigureAwait(false);
        }
    }
}