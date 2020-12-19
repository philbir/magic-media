using MagicMedia.Api.DevTokenAuthentication;
using Microsoft.AspNetCore.Authentication;

namespace MagicMedia
{
    public static partial class AuthenticationExtensions
    {
        static partial void SetupDevelopmentAuthentication(AuthenticationBuilder builder)
        {
            builder.AddDevToken();
        }
    }
}
