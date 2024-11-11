// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Netpack.GatewayDashboard.Model.BrowserStorage;

public class SessionBrowserStorage : BrowserStorageBase, ISessionStorage
{
    public SessionBrowserStorage(ProtectedSessionStorage protectedSessionStorage) : base(protectedSessionStorage)
    {
    }
}
