using System;
using Microsoft.AspNetCore.Authentication;

namespace MagicMedia.Api.DevTokenAuthentication;

public static class DevTokenExtensions
{
    public static AuthenticationBuilder AddDevToken(this AuthenticationBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        return builder
            .AddScheme<DevTokenAuthenticationOptions, DevTokenAuthorizationHandler>(
                DevTokenDefaults.AuthenticationScheme,
                DevTokenDefaults.AuthenticationScheme,
                _ => { });
    }
}
