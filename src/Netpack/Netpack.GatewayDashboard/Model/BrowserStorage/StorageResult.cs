// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Netpack.GatewayDashboard.Model.BrowserStorage;

public readonly record struct StorageResult<T>(bool Success, T? Value);
