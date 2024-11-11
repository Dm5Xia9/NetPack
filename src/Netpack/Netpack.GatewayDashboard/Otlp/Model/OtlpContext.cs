// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Configuration;

namespace Netpack.GatewayDashboard.Otlp.Model;

public sealed class OtlpContext
{
    public required ILogger Logger { get; init; }
    public required TelemetryLimitOptions Options { get; init; }
}
