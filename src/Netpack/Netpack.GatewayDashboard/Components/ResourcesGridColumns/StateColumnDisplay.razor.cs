// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Model;
using Netpack.GatewayDashboard.Otlp.Storage;
using Netpack.GatewayDashboard.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Netpack.GatewayDashboard.Components.ResourcesGridColumns;

public partial class StateColumnDisplay
{
    [Parameter, EditorRequired]
    public required ResourceViewModel Resource { get; set; }

    [Parameter, EditorRequired]
    public required Dictionary<ApplicationKey, int>? UnviewedErrorCounts { get; set; }

    [Inject]
    public required IStringLocalizer<Columns> Loc { get; init; }
}
