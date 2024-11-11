// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Model;
using Netpack.GatewayDashboard.Otlp.Model;
using Netpack.GatewayDashboard.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Netpack.GatewayDashboard.Components;

public partial class TraceActions : ComponentBase
{
    private static readonly Icon s_viewDetailsIcon = new Icons.Regular.Size16.Info();
    private static readonly Icon s_structuredLogsIcon = new Icons.Regular.Size16.SlideTextSparkle();

    private AspireMenuButton? _menuButton;

    [Inject]
    public required IStringLocalizer<Resources.ControlsStrings> ControlsLoc { get; set; }

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Parameter]
    public required OtlpTrace Trace { get; set; }

    private readonly List<MenuButtonItem> _menuItems = new();

    protected override void OnParametersSet()
    {
        _menuItems.Clear();

        _menuItems.Add(new MenuButtonItem
        {
            Text = ControlsLoc[nameof(Resources.ControlsStrings.ActionViewDetailsText)],
            Icon = s_viewDetailsIcon,
            OnClick = () =>
            {
                NavigationManager.NavigateTo(DashboardUrls.TraceDetailUrl(Trace.TraceId));
                return Task.CompletedTask;
            }
        });
        _menuItems.Add(new MenuButtonItem
        {
            Text = ControlsLoc[nameof(Resources.ControlsStrings.ActionStructuredLogsText)],
            Icon = s_structuredLogsIcon,
            OnClick = () =>
            {
                NavigationManager.NavigateTo(DashboardUrls.StructuredLogsUrl(traceId: Trace.TraceId));
                return Task.CompletedTask;
            }
        });
    }
}
