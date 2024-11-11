// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Authentication;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Proto.Collector.Trace.V1;

namespace Netpack.GatewayDashboard.Otlp.Grpc;

[Authorize(Policy = OtlpAuthorization.PolicyName)]
[SkipStatusCodePages]
public class OtlpGrpcTraceService : TraceService.TraceServiceBase
{
    private readonly OtlpTraceService _traceService;

    public OtlpGrpcTraceService(OtlpTraceService traceService)
    {
        _traceService = traceService;
    }

    public override Task<ExportTraceServiceResponse> Export(ExportTraceServiceRequest request, ServerCallContext context)
    {
        return Task.FromResult(_traceService.Export(request));
    }
}
