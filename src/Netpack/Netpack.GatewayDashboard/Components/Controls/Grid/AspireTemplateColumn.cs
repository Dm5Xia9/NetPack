// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Netpack.GatewayDashboard.Components.Resize;

namespace Netpack.GatewayDashboard.Components.Controls.Grid;

public class AspireTemplateColumn<TGridItem> : TemplateColumn<TGridItem>, IAspireColumn
{
    [Parameter]
    public GridColumnManager? ColumnManager { get; set; }

    [Parameter]
    public string? ColumnId { get; set; }

    [Parameter]
    public bool UseCustomHeaderTemplate { get; set; } = true;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (UseCustomHeaderTemplate)
        {
            HeaderCellItemTemplate = AspireFluentDataGridHeaderCell.RenderHeaderContent(Grid);
        }
    }

    protected override bool ShouldRender()
    {
        return (ColumnManager is null || ColumnId is null || ColumnManager.IsColumnVisible(ColumnId)) && base.ShouldRender();
    }
}
