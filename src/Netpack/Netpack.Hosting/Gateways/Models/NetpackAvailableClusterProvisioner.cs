// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Netpack.Hosting.Gateways.Models;

internal class NetpackAvailableClusterProvisioner
{
    public required string ClusterName { get; set; }
    public ClusterType ClusterType { get; set; }
    public Dictionary<string, string> Properies { get; set; } = [];

    public List<NetpackOpportunity> Opportunities { get; set; } = [];
}
