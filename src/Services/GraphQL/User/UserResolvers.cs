using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class UserResolvers
    {
        private readonly IPersonService _personService;
        private readonly IUserService _userService;
        private readonly IAlbumService _albumService;

        public UserResolvers(
            IPersonService personService,
            IUserService userService,
            IAlbumService albumService)
        {
            _personService = personService;
            _userService = userService;
            _albumService = albumService;
        }

        public async Task<Person?> GetPersonAsync(User user, CancellationToken cancellationToken)
        {
            if (user.PersonId.HasValue)
            {
                return await _personService.GetByIdAsync(user.PersonId.Value, cancellationToken);
            }

            return null;
        }

        public IEnumerable<string> GetPermissions(User user)
        {
            return _userService.GetPermissions(user);
        }

        public Task<IEnumerable<Album>> GetSharedAlbumsAsync(
            User user,
            CancellationToken cancellationToken)
        {
            return _userService.GetSharedAlbumsAsync(user.Id, cancellationToken);
        }

        public async Task<IEnumerable<Album>> GetInAlbumAsync(
            User user,
            CancellationToken cancellationToken)
        {

            if ( user.PersonId.HasValue)
            {
                return await _albumService.GetWithPersonAsync(
                    user.PersonId.Value,
                    cancellationToken);
            }

            return Array.Empty<Album>();
        }


        public async Task<IEnumerable<Guid>> GetAuthorizedOnMediaIdsAsync(
            User user,
            CancellationToken cancellationToken)
        {
            return await _userService.GetAuthorizedOnMediaIdsAsync(user.Id, cancellationToken);
        }

        public async Task<int> GetAuthorizedOnMediaCountAsync(
            User user,
            CancellationToken cancellationToken)
        {
            return (await _userService.GetAuthorizedOnMediaIdsAsync(user.Id, cancellationToken)).
                Count();
        }
    }
}
