using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;

namespace MagicMedia.Authorization;

public static class AuthorizationPolicies
{
    public static class Names
    {
        public const string ApiAccess = nameof(ApiAccessPolicy);

        public const string MediaView = nameof(MediaViewPolicy);
        public const string MediaEdit = nameof(MediaEditPolicy);
        public const string MediaDestroy = nameof(MediaDestroyPolicy);
        public const string MediaDownload = nameof(MediaDownloadPolicy);

        public const string FaceView = nameof(FaceViewPolicy);
        public const string FaceEdit = nameof(FaceEditPolicy);

        public const string AlbumView = nameof(AlbumViewPolicy);
        public const string AlbumEdit = nameof(AlbumEditPolicy);
        public const string AlbumDelete = nameof(AlbumDeletePolicy);

        public const string PersonEdit = nameof(PersonEditPolicy);
        public const string PersonDelete = nameof(PersonDeletePolicy);

        public const string UserView = nameof(UserViewPolicy);
        public const string UserEdit = nameof(UserEditPolicy);

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

    public static AuthorizationPolicy MediaDestroyPolicy
    {
        get
        {
            return new AuthorizationPolicyBuilder()
                .RequirePermission(Permissions.Media.Destroy)
                .Build();
        }
    }

    public static AuthorizationPolicy MediaDownloadPolicy
    {
        get
        {
            return new AuthorizationPolicyBuilder()
                .RequirePermission(Permissions.Media.Download)
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

    public static AuthorizationPolicy AlbumDeletePolicy
    {
        get
        {
            return new AuthorizationPolicyBuilder()
                .RequirePermission(Permissions.Album.Delete)
                .Build();
        }
    }

    public static AuthorizationPolicy PersonDeletePolicy
    {
        get
        {
            return new AuthorizationPolicyBuilder()
                .RequirePermission(Permissions.Person.Delete)
                .Build();
        }
    }

    public static AuthorizationPolicy PersonEditPolicy
    {
        get
        {
            return new AuthorizationPolicyBuilder()
                .RequirePermission(Permissions.Person.Edit)
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

    public static AuthorizationPolicy UserViewPolicy
    {
        get
        {
            return new AuthorizationPolicyBuilder()
                .RequirePermission(Permissions.User.View)
                .Build();
        }
    }

    public static AuthorizationPolicy UserEditPolicy
    {
        get
        {
            return new AuthorizationPolicyBuilder()
                .RequirePermission(Permissions.User.Edit)
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
