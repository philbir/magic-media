using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using Microsoft.Extensions.Caching.Memory;

namespace MagicMedia.Security
{
    public class UserService : IUserService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IMemoryCache _memoryCache;
        private readonly IAlbumMediaIdResolver _albumMediaIdResolver;

        public UserService(
            IMediaStore mediaStore,
            IMemoryCache memoryCache,
            IAlbumMediaIdResolver albumMediaIdResolver)
        {
            _mediaStore = mediaStore;
            _memoryCache = memoryCache;
            _albumMediaIdResolver = albumMediaIdResolver;
        }

        public async Task<User> TryGetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _mediaStore.Users.TryGetByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _mediaStore.Users.GetAllAsync(cancellationToken);
        }

        public async Task<User> TryGetByPersonIdAsync(
            Guid personId,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.Users.TryGetByPersonIdAsync(personId, cancellationToken);
        }

        public async Task<IEnumerable<Album>> GetSharedAlbumsAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.Albums.GetSharedWithUserIdAsync(userId, cancellationToken);
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

        public async Task<IEnumerable<Guid>> GetAuthorizedOnAlbumIdsAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var cachekey = $"auth_on_album_{userId}";

            IEnumerable<Guid> ids = await _memoryCache.GetOrCreateAsync(
                cachekey,
                async (e) =>
                {
                    e.AbsoluteExpiration = DateTime.Now.AddMinutes(15);
                    IEnumerable<Album> albums = await GetSharedAlbumsAsync(userId, cancellationToken);
                    return albums.Select(x => x.Id);
                });

            return ids;
        }

        public async Task<IEnumerable<Guid>> GetAuthorizedOnPersonIdsAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var cachekey = $"auth_on_person_{userId}";

            IEnumerable<Guid> ids = await _memoryCache.GetOrCreateAsync(
                cachekey,
                async (e) =>
                {
                    e.AbsoluteExpiration = DateTime.Now.AddMinutes(15);
                    return await GetAuthorizedOnPersonIdsInternalAsync(userId, cancellationToken);
                });

            return ids;
        }

        private async Task<IEnumerable<Guid>> GetAuthorizedOnMediaIdsInternalAsync(
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


        private async Task<IEnumerable<Guid>> GetAuthorizedOnPersonIdsInternalAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            IEnumerable<Guid> mediaIds = await GetAuthorizedOnMediaIdsAsync(
                userId,
                cancellationToken);

            IEnumerable<Guid> persons = await _mediaStore.Faces.GetPersonsIdsByMediaAsync(
                mediaIds,
                cancellationToken);

            return persons;
        }

        public async Task<User> CreateFromPersonAsync(CreateUserFromPersonRequest request, CancellationToken cancellationToken)
        {
            //TODO: Validate if there is allready a user for this person

            Person person = await _mediaStore.Persons.GetByIdAsync(request.PersonId, cancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = person.Name,
                Email = request.Email,
                PersonId = person.Id
            };

            await _mediaStore.Users.AddAsync(user, cancellationToken);

            return user;
        }
    }
}
