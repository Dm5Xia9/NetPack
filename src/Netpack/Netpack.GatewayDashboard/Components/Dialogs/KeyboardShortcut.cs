// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Netpack.GatewayDashboard.Components.Dialogs;

public record KeyboardShortcutCategory(string Category, List<KeyboardShortcut> Shortcuts);

public record KeyboardShortcut(string[] Keys, string Description);
