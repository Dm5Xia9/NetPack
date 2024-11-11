// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Otlp.Model;
using Netpack.GatewayDashboard.Otlp.Storage;
using OpenTelemetry.Proto.Collector.Metrics.V1;

namespace Netpack.GatewayDashboard.Otlp;

public sealed class OtlpMetricsService
{
    private readonly ILogger<OtlpMetricsService> _logger;
    private readonly TelemetryRepository _telemetryRepository;

    public OtlpMetricsService(ILogger<OtlpMetricsService> logger, TelemetryRepository telemetryRepository)
    {
        _logger = logger;
        _telemetryRepository = telemetryRepository;
    }

    public ExportMetricsServiceResponse Export(ExportMetricsServiceRequest request)
    {
        var addContext = new AddContext();
        _telemetryRepository.AddMetrics(addContext, request.ResourceMetrics);

        _logger.LogDebug("Processed metrics export. Failure count: {FailureCount}", addContext.FailureCount);

        return new ExportMetricsServiceResponse
        {
            PartialSuccess = new ExportMetricsPartialSuccess
            {
                RejectedDataPoints = addContext.FailureCount
            }
        };
    }
}
