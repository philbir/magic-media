using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;

namespace MagicMedia.Authorization
{
    public static class AuthorizationPolicies
    {
        public static class Names
        {
            public const string ApiAccess = nameof(ApiAccessPolicy);

            public const string MediaView = nameof(MediaViewPolicy);
            public const string MediaEdit = nameof(MediaEditPolicy);

            public const string FaceView = nameof(FaceViewPolicy);
            public const string FaceEdit = nameof(FaceEditPolicy);


            public const string AlbumView = nameof(AlbumViewPolicy);
            public const string AlbumEdit = nameof(AlbumEditPolicy);

            public const string ManageUsers = nameof(ManageUsersPolicy);

            public const string ImageAI = nameof(ImageAIPolicy);
        }

        public static AuthorizationPolicy MediaViewPolicy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .AddRequirements(new AuhorizedOnMediaRequirement())
                    .Build();
            }
        }

        public static AuthorizationPolicy MediaEditPolicy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.Media.Edit)
                    .Build();
            }
        }

        public static AuthorizationPolicy AlbumViewPolicy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .AddRequirements(new AuhorizedOnAlbumRequirement())
                    .Build();
            }
        }

        public static AuthorizationPolicy AlbumEditPolicy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.Album.Edit)
                    .Build();
            }
        }


        public static AuthorizationPolicy FaceViewPolicy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .AddRequirements(new AuhorizedOnFaceRequirement())
                    .Build();
            }
        }

        public static AuthorizationPolicy FaceEditPolicy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.Face.Edit)
                    .Build();
            }
        }

        public static AuthorizationPolicy ManageUsersPolicy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.User.Manage)
                    .Build();
            }
        }

        public static AuthorizationPolicy ImageAIPolicy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequireClaim("scope", "api.magic.imageai")
                    .AddAuthenticationSchemes("jwt")
                    .Build();
            }
        }

        public static AuthorizationPolicy ApiAccessPolicy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            }
        }
    }
}
