// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.ApplicationModel;
using Netpack.Hosting.Gateways;
using Netpack.Hosting.Gateways.Models;
using Netpack.Hosting.Models;
using Netpack.Hosting.Orcestraction;

namespace Netpack.Hosting.Clusters;

/// <summary>
/// 
/// </summary>
public abstract class NetpackClusterResource : NetpackResource, IResourceWithConnectionString, IResourceWithParent<NetpackGatewayResource>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="gatewayResource"></param>
    protected NetpackClusterResource(string name, NetpackGatewayResource gatewayResource) : base(name)
    {
        Parent = gatewayResource;
    }
    /// <inheritdoc/>

    public abstract ReferenceExpression ConnectionStringExpression { get; }
    /// <inheritdoc/>

    public abstract ClusterType ClusterType { get; }
    /// <inheritdoc/>
    public NetpackGatewayResource Parent { get; }
    /// <inheritdoc/>

    public NetpackAvailableClusterProvisioner? Provisioner { get; private set; }
    /// <inheritdoc/>

    public override Task AfterEndpointsAllocatedAsync(ResourceState state, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    /// <inheritdoc/>

    public override async Task AfterResourcesCreatedAsync(ResourceState state, CancellationToken cancellationToken)
    {
        await state.WaitForResourceAsync(Parent.Name, cancellationToken, KnownResourceStates.Running).ConfigureAwait(false);

        await state.Try(async () =>
        {
            await state.SetState(KnownResourceStateStyles.Info, "Подключение").ConfigureAwait(false);
            await using var scope = state.Scope().ConfigureAwait(false);

            await state.Try(async () =>
            {
                await state.SetState(KnownResourceStateStyles.Info, "Настройка").ConfigureAwait(false);
                await using var scope = state.Scope("Настроено").ConfigureAwait(false);

                await state.Try(async () =>
                {
                    await state.SetState(KnownResourceStateStyles.Info, "Проверка доступа").ConfigureAwait(false);
                    await using var scope = state.Scope("Разрешено").ConfigureAwait(false);
                    Provisioner = Parent.Provisions
                        .Clusters.Single(p => p.ClusterName == Name &&
                        p.ClusterType == ClusterType);
                }, "Нет доступа").ConfigureAwait(false);

                await Configuration(state, cancellationToken).ConfigureAwait(false);
            }, "Ошибка конфигурирования").ConfigureAwait(false);

            await Connect(state, cancellationToken).ConfigureAwait(false);
        }, "Ошибка подключения").ConfigureAwait(false);
    }
    /// <inheritdoc/>

    protected abstract Task Configuration(ResourceState state, CancellationToken cancellationToken);
    /// <inheritdoc/>

    protected abstract Task Connect(ResourceState state, CancellationToken cancellationToken);
    /// <inheritdoc/>

    public override Task BeforeStartAsync(ResourceState state, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
