// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Netpack.GatewayDashboard.Otlp.Model;

namespace Netpack.GatewayDashboard.Model;

public interface IInstrumentUnitResolver
{
    string ResolveDisplayedUnit(OtlpInstrumentSummary instrument, bool titleCase, bool pluralize);
}