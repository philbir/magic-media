using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Security
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly IUserContextFactory _userContextFactory;

        public UserAuthorizationService(IUserContextFactory userContextFactory)
        {
            _userContextFactory = userContextFactory;
        }

        public async Task<UserResourceAccessInfo> GetAuthorizedOnAsync(ProtectedResourceType type, CancellationToken cancellationToken)
        {
            var info = new UserResourceAccessInfo { Type = type };
            IUserContext userContext = await _userContextFactory.CreateAsync(cancellationToken);

            switch (type)
            {
                case ProtectedResourceType.Media:
                    if (userContext.HasPermission(Permissions.Media.ViewAll))
                    {
                        info.ViewAll = true;
                    }
                    else
                    {
                        info.Ids = await userContext.GetAuthorizedMediaAsync(cancellationToken);
                    }
                    break;
                case ProtectedResourceType.Album:
                    if (userContext.HasPermission(Permissions.Album.ViewAll))
                    {
                        info.ViewAll = true;
                    }
                    else
                    {
                        info.Ids = await userContext.GetAuthorizedAlbumAsync(cancellationToken);
                    }
                    break;
                case ProtectedResourceType.Person:
                    if (userContext.HasPermission(Permissions.Person.ViewAll))
                    {
                        info.ViewAll = true;
                    }
                    else
                    {
                        info.Ids = await userContext.GetAuthorizedPersonsAsync(cancellationToken);
                    }
                    break;
            }

            return info;
        }
    }
}
