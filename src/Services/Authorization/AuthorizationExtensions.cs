using MagicMedia.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorization(
            this IServiceCollection services)
        {
            services.AddAuthorization(o =>
            {
                o.AddPolicy(
                    AuthorizationPolicies.Names.ApiAccess,
                    AuthorizationPolicies.ApiAccessPolicy);

                o.AddPolicy(
                    AuthorizationPolicies.Names.MediaView,
                    AuthorizationPolicies.MediaViewPolicy);

                o.AddPolicy(
                    AuthorizationPolicies.Names.MediaEdit,
                    AuthorizationPolicies.MediaEditPolicy);

                o.AddPolicy(
                    AuthorizationPolicies.Names.FaceView,
                    AuthorizationPolicies.FaceViewPolicy);

                o.AddPolicy(
                    AuthorizationPolicies.Names.FaceEdit,
                    AuthorizationPolicies.FaceEditPolicy);

                o.AddPolicy(
                    AuthorizationPolicies.Names.AlbumView,
                    AuthorizationPolicies.AlbumViewPolicy);

                o.AddPolicy(
                    AuthorizationPolicies.Names.AlbumEdit,
                    AuthorizationPolicies.AlbumEditPolicy);

                o.AddPolicy(
                    AuthorizationPolicies.Names.ManageUsers,
                    AuthorizationPolicies.ManageUsersPolicy);
            });

            services.AddSingleton<IAuthorizationHandler, MediaAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AlbumAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, FaceAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }
    }
}
