// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Components.Controls.Chart;
using Netpack.GatewayDashboard.Otlp.Model;

namespace Netpack.GatewayDashboard.Model;

public sealed class ExemplarsDialogViewModel
{
    public required List<ChartExemplar> Exemplars { get; init; }
    public required List<OtlpApplication> Applications { get; init; }
    public required OtlpInstrumentSummary Instrument { get; init; }
}
