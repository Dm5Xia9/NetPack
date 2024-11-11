// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Components.Resize;

namespace Netpack.GatewayDashboard.Components.Controls.Grid;

internal interface IAspireColumn
{
    public GridColumnManager? ColumnManager { get; set; }

    public string? ColumnId { get; set; }

    public bool UseCustomHeaderTemplate { get; set; }
}
