// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.Hosting.Gateways.Models;

namespace Netpack.Hosting.Gateways;

internal class GatewayClient
{
    public static NetpackGatewayAvailableProvisions GetProvisions()
    {
        return new NetpackGatewayAvailableProvisions
        {
            Clusters =
            [
                new NetpackAvailableClusterProvisioner
                {
                    ClusterName = "dev",
                    ClusterType = ClusterType.Kube,
                    Properies = new Dictionary<string, string>
                    {
                        { ClusterProperties.KubeconfigUrl, "C:\\Users\\user\\Documents\\ass\\config (8).yaml" }
                    }
                },
            ]
        };
    }
}
