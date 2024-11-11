// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Netpack.Hosting.Models;
using Netpack.Hosting.Orcestraction;
using Netpack.Hosting.Processes;
using Netpack.Hosting.Processes.Observer;

namespace Netpack.Hosting;

internal class NetpackLifecycleHook : IDistributedApplicationLifecycleHook
{
    private readonly ResourceNotificationService _resourceNotificationService;
    private readonly ResourceLoggerService _resourceLoggerService;
    private readonly IProcessStorage _processStorage;
    private readonly ProcessObserver _processObserver;

    public NetpackLifecycleHook(ResourceNotificationService resourceNotificationService, ResourceLoggerService resourceLoggerService, IProcessStorage processStorage, ProcessObserver processObserver)
    {
        _resourceNotificationService = resourceNotificationService;
        _resourceLoggerService = resourceLoggerService;
        _processStorage = processStorage;
        _processObserver = processObserver;
    }

    Task IDistributedApplicationLifecycleHook.BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken)
    {
        _processObserver.Start();
        var netpackResources = appModel.Resources.OfType<INetpackResource>();

        _ = netpackResources
            .Select(p => new ResourceState(p,
                _resourceNotificationService,
                _resourceLoggerService,
                _processStorage))
            .Select(p => Task.Run(() =>
            {
                _ = p.Resource.BeforeStartAsync(p, cancellationToken);
            }));

        return Task.CompletedTask;
    }

    Task IDistributedApplicationLifecycleHook.AfterEndpointsAllocatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken)
    {
        var netpackResources = appModel.Resources.OfType<INetpackResource>();

        _ = netpackResources
             .Select(p => new ResourceState(p,
                 _resourceNotificationService,
                 _resourceLoggerService,
                 _processStorage))
             .Select(p => Task.Run(() =>
             {
                 _ = p.Resource.AfterEndpointsAllocatedAsync(p, cancellationToken);
             }));

        return Task.CompletedTask;
    }

    async Task IDistributedApplicationLifecycleHook.AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken)
    {
        var netpackResources = appModel.Resources.OfType<INetpackResource>();

        await netpackResources
             .Select(p => new ResourceState(p,
                 _resourceNotificationService,
                 _resourceLoggerService,
                 _processStorage))
             .ProcessNodes(async p =>
             {
                 try
                 {
                     await p.Resource.AfterResourcesCreatedAsync(p, cancellationToken).ConfigureAwait(false);
                 }
                 catch (Exception)
                 {
                     await p.SetState(KnownResourceStateStyles.Error, KnownResourceStates.FailedToStart).ConfigureAwait(false);
                 }
             }).ConfigureAwait(false);
    }

}

/// <summary>
/// 
/// </summary>
public static class NetpackResourceExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="states"></param>
    /// <param name="processNode"></param>
    /// <returns></returns>
    public static async Task ProcessNodes(this IEnumerable<ResourceState> states, Func<ResourceState, Task> processNode)
    {
        foreach (var root in states.Where(p => p.Resource is not IResourceWithParent))
        {
            await ProcessNodeRecursive(root, states, processNode).ConfigureAwait(false);
        }
    }

    private static async Task ProcessNodeRecursive(ResourceState node, IEnumerable<ResourceState> allNodes, Func<ResourceState, Task> processNode)
    {
        await processNode(node).ConfigureAwait(false);

        foreach (var child in allNodes)
        {
            if (child.Resource is IResourceWithParent rp && rp.Parent == node.Resource)
            {
                await ProcessNodeRecursive(child, allNodes, processNode).ConfigureAwait(false);
            }
        }
    }
}
