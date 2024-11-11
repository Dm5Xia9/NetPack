﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Configuration;
using Microsoft.Extensions.Options;

namespace Netpack.GatewayDashboard.Model;

internal sealed class DashboardClientStatus(IOptions<DashboardOptions> dashboardOptions) : IDashboardClientStatus
{
    public bool IsEnabled => dashboardOptions.Value.ResourceServiceClient.GetUri() is not null;
}
