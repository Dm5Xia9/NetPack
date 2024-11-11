// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Components.Controls;

namespace Netpack.GatewayDashboard.Model;

public sealed class TelemetryPropertyViewModel : IPropertyGridItem
{
    public required string Name { get; init; }
    public required string Value { get; init; }
    public required object Key { get; init; }
}
