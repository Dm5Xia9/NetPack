// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Authentication;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Proto.Collector.Metrics.V1;

namespace Netpack.GatewayDashboard.Otlp.Grpc;

[Authorize(Policy = OtlpAuthorization.PolicyName)]
[SkipStatusCodePages]
public class OtlpGrpcMetricsService : MetricsService.MetricsServiceBase
{
    private readonly OtlpMetricsService _metricsService;

    public OtlpGrpcMetricsService(OtlpMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    public override Task<ExportMetricsServiceResponse> Export(ExportMetricsServiceRequest request, ServerCallContext context)
    {
        return Task.FromResult(_metricsService.Export(request));
    }
}
