// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Model;

namespace Netpack.GatewayDashboard.Extensions;

internal static class ResourceViewModelExtensions
{
    public static bool IsHiddenState(this ResourceViewModel resource)
    {
        return resource.KnownState is KnownResourceState.Hidden;
    }

    public static bool IsRunningState(this ResourceViewModel resource)
    {
        return resource.KnownState is KnownResourceState.Running;
    }

    public static bool IsFinishedState(this ResourceViewModel resource)
    {
        return resource.KnownState is KnownResourceState.Finished;
    }

    public static bool IsStopped(this ResourceViewModel resource)
    {
        return resource.KnownState is KnownResourceState.Exited or KnownResourceState.Finished or KnownResourceState.FailedToStart;
    }

    public static bool IsUnusableTransitoryState(this ResourceViewModel resource)
    {
        return resource.KnownState is KnownResourceState.Starting or KnownResourceState.Building or KnownResourceState.Waiting or KnownResourceState.Stopping;
    }

    public static bool IsRuntimeUnhealthy(this ResourceViewModel resource)
    {
        return resource.KnownState is KnownResourceState.RuntimeUnhealthy;
    }

    public static bool IsUnknownState(this ResourceViewModel resource)
    {
        return resource.KnownState is KnownResourceState.Unknown;
    }

    public static bool HasNoState(this ResourceViewModel resource)
    {
        return string.IsNullOrEmpty(resource.State);
    }
}