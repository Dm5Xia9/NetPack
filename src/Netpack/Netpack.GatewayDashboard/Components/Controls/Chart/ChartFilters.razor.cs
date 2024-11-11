// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Model;
using Netpack.GatewayDashboard.Otlp.Model;
using Microsoft.AspNetCore.Components;

namespace Netpack.GatewayDashboard.Components;

public partial class ChartFilters
{
    [Parameter, EditorRequired]
    public required OtlpInstrumentData Instrument { get; set; }

    [Parameter, EditorRequired]
    public required InstrumentViewModel InstrumentViewModel { get; set; }

    [Parameter, EditorRequired]
    public required List<DimensionFilterViewModel> DimensionFilters { get; set; }

    public bool ShowCounts { get; set; }

    protected override void OnInitialized()
    {
        InstrumentViewModel.DataUpdateSubscriptions.Add(() =>
        {
            ShowCounts = InstrumentViewModel.ShowCount;
            return Task.CompletedTask;
        });
    }

    private void ShowCountChanged()
    {
        InstrumentViewModel.ShowCount = ShowCounts;
    }
}
