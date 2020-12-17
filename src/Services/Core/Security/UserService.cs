using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using Microsoft.Extensions.Caching.Memory;

namespace MagicMedia.Security
{
    public class UserService : IUserService
    {
        private readonly IUserStore _userStore;
        private readonly IAlbumStore _albumStore;
        private readonly IPersonService _personService;
        private readonly IMemoryCache _memoryCache;
        private readonly IAlbumMediaIdResolver _albumMediaIdResolver;

        public UserService(
            IUserStore userStore,
            IAlbumStore albumStore,
            IPersonService personService,
            IMemoryCache memoryCache,
            IAlbumMediaIdResolver albumMediaIdResolver)
        {
            _userStore = userStore;
            _albumStore = albumStore;
            _personService = personService;
            _memoryCache = memoryCache;
            _albumMediaIdResolver = albumMediaIdResolver;
        }

        public async Task<User> TryGetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _userStore.TryGetByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _userStore.GetAllAsync(cancellationToken);
        }

        public async Task<User> TryGetByPersonIdAsync(
            Guid personId,
            CancellationToken cancellationToken)
        {
            return await _userStore.TryGetByPersonIdAsync(personId, cancellationToken);
        }

        public async Task<IEnumerable<Album>> GetSharedAlbumsAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _albumStore.GetSharedByUserIdAsync(userId, cancellationToken);
        }

        public async Task<IEnumerable<Guid>> GetAuthorizedOnMediaIdsAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var cachekey = $"auth_on_media_{userId}";

            IEnumerable<Guid> ids = await _memoryCache.GetOrCreateAsync(
                cachekey,
                async (e) =>
                 {
                     e.AbsoluteExpiration = DateTime.Now.AddMinutes(15);
                     return await GetAuthorizedOnMediaIdsInternalAsync(userId, cancellationToken);
                 });

            return ids;
        }

        public async Task<IEnumerable<Guid>> GetAuthorizedOnMediaIdsInternalAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            IEnumerable<Album> albums = await GetSharedAlbumsAsync(userId, cancellationToken);

            var ids = new HashSet<Guid>();

            foreach (Album? album in albums)
            {
                IEnumerable<Guid>? albumMediaIds = await _albumMediaIdResolver.GetMediaIdsAsync(album, cancellationToken);

                ids.UnionWith(albumMediaIds);
            }

            return ids;
        }

        public async Task<User> CreateFromPersonAsync(CreateUserFromPersonRequest request, CancellationToken cancellationToken)
        {
            //TODO: Validate if there is allready a user for this person

            Person person = await _personService.GetByIdAsync(request.PersonId, cancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = person.Name,
                Email = request.Email,
                PersonId = person.Id
            };

            await _userStore.AddAsync(user, cancellationToken);

            return user;
        }
    }
}
