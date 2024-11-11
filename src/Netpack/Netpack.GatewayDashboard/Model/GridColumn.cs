// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Netpack.GatewayDashboard.Model;

public record GridColumn(string Name, string? DesktopWidth, string? MobileWidth = null, Func<bool>? IsVisible = null);
