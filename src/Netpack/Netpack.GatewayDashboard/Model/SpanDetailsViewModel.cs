// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Otlp.Model;

namespace Netpack.GatewayDashboard.Model;

public sealed class SpanDetailsViewModel
{
    public required OtlpSpan Span { get; init; }
    public required List<TelemetryPropertyViewModel> Properties { get; init; }
    public required List<SpanLinkViewModel> Links { get; init; }
    public required List<SpanLinkViewModel> Backlinks { get; init; }
    public required string Title { get; init; }
    public required List<OtlpApplication> Applications { get; init; }
}
