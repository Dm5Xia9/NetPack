// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Netpack.Hosting.Processes;
using Netpack.Hosting.Processes.Observer;

namespace Netpack.Hosting.Gateways;

/// <summary>
/// 
/// </summary>
public static class Extensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IResourceBuilder<NetpackGatewayResource> AddNetpack(this IDistributedApplicationBuilder builder, string name)
    {
        _ = builder.Services.Configure<ProcessObserverOptions>(p =>
        {
            p.WorkFolderPath = Directory.CreateTempSubdirectory("netpack").FullName;
        });
        builder.Services.TryAddSingleton<IProcessStorage, ProcessStorage>();
        builder.Services.TryAddSingleton<ProcessObserver>();
        builder.Services.TryAddLifecycleHook<NetpackLifecycleHook>();
        _ = builder.Services.AddHostedService<ProcessesHostedService>();
        return builder.AddResource(new NetpackGatewayResource($"{name}"))
            .WithInitialState(new CustomResourceSnapshot
            {
                ResourceType = "Gateway",
                Properties = []
            });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="gatewayDiscoveryPort"></param>
    /// <param name="gatewayDiscoveryImage"></param>
    public static void AddNetpackServer(this IDistributedApplicationBuilder builder,
        string name,
        int gatewayDiscoveryPort = 9898,
        string gatewayDiscoveryImage = "netpack/gd")
    {
        var networkName = name.ToNetworkName();
        _ = builder.AddContainer("GatewayDiscovery", gatewayDiscoveryImage)
            .WithEnvironment($"GATEWAY_{networkName}", "aboba")
            .WithHttpEndpoint(gatewayDiscoveryPort, targetPort: 8080);
    }
}

internal static class GatewayExtensions
{
    public static string ToNetworkName(this string name)
    {
        return name.Replace("#", "");
    }
}
