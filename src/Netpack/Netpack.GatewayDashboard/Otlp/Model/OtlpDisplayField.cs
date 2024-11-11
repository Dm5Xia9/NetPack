// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Netpack.GatewayDashboard.Otlp.Model;

public class OtlpDisplayField
{
    public required string DisplayName { get; init; }
    public required object Key { get; init; }
    public required string Value{ get; init; }
}
