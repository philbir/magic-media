using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace MagicMedia.Identity.AuthProviders
{
    public static class AuthProviderEventsExtensions
    {
        public static Task HandleRemoteFailure(this RemoteFailureContext context)
        {
            Log.Error(
                context.Failure,
                "External authentication remote failure. {Scheme}",
                context.Scheme.Name);

            context.Response.RedirectExternalError(context.Scheme.Name, context.Properties);
            context.HandleResponse();

            return Task.CompletedTask;
        }


        public static Task HandleAccessDenied(this AccessDeniedContext context)
        {
            Log.Error(
                "External authentication access denied. {Scheme}",
                context.Scheme.Name);

            context.Response.RedirectExternalError(context.Scheme.Name, context.Properties);
            context.HandleResponse();

            return Task.CompletedTask;
        }

        public static void RedirectExternalError(
            this HttpResponse response,
            string scheme,
            AuthenticationProperties? properties)
        {
            response.Redirect(
                $"/External/Error/{scheme}");
        }
    }
}
