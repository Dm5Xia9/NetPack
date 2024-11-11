// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using System.Text.Encodings.Web;
using Netpack.GatewayDashboard.Authentication.OtlpApiKey;
using Netpack.GatewayDashboard.Authentication.Connection;
using Netpack.GatewayDashboard.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.Extensions.Options;

namespace Netpack.GatewayDashboard.Authentication;

public sealed class OtlpCompositeAuthenticationHandler(
    IOptionsMonitor<DashboardOptions> dashboardOptions,
    IOptionsMonitor<OtlpCompositeAuthenticationHandlerOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
        : AuthenticationHandler<OtlpCompositeAuthenticationHandlerOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var options = dashboardOptions.CurrentValue;

        foreach (var scheme in GetRelevantAuthenticationSchemes())
        {
            var result = await Context.AuthenticateAsync(scheme).ConfigureAwait(false);

            if (result.Failure is not null)
            {
                return result;
            }
        }

        var id = new ClaimsIdentity([new Claim(OtlpAuthorization.OtlpClaimName, bool.TrueString)]);

        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(id), Scheme.Name));

        IEnumerable<string> GetRelevantAuthenticationSchemes()
        {
            yield return ConnectionTypeAuthenticationDefaults.AuthenticationSchemeOtlp;

            if (options.Otlp.AuthMode is OtlpAuthMode.ApiKey)
            {
                yield return OtlpApiKeyAuthenticationDefaults.AuthenticationScheme;
            }
            else if (options.Otlp.AuthMode is OtlpAuthMode.ClientCertificate)
            {
                yield return CertificateAuthenticationDefaults.AuthenticationScheme;
            }
        }
    }
}

public static class OtlpCompositeAuthenticationDefaults
{
    public const string AuthenticationScheme = "OtlpComposite";
}

public sealed class OtlpCompositeAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
}
