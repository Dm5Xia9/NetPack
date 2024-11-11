// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Authentication;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Proto.Collector.Logs.V1;

namespace Netpack.GatewayDashboard.Otlp.Grpc;

[Authorize(Policy = OtlpAuthorization.PolicyName)]
[SkipStatusCodePages]
public class OtlpGrpcLogsService : LogsService.LogsServiceBase
{
    private readonly OtlpLogsService _logsService;

    public OtlpGrpcLogsService(OtlpLogsService logsService)
    {
        _logsService = logsService;
    }

    public override Task<ExportLogsServiceResponse> Export(ExportLogsServiceRequest request, ServerCallContext context)
    {
        return Task.FromResult(_logsService.Export(request));
    }
}
