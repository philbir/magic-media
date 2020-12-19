using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using Microsoft.Extensions.Caching.Memory;

namespace MagicMedia.Security
{
    public class UserService : IUserService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IMemoryCache _memoryCache;
        private readonly IAlbumMediaIdResolver _albumMediaIdResolver;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15);

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
            var cachekey = $"mm_user_{id}";

            User? user = await _memoryCache.GetOrCreateAsync(
                cachekey,
                async (e) =>
                {
                    e.AbsoluteExpiration = DateTime.Now.Add(_cacheExpiration);

                    return await _mediaStore.Users.TryGetByIdAsync(id, cancellationToken);
                });

            return user;
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

        public IEnumerable<string> GetPermissions(User user)
        {
            var permissions = new HashSet<string>();

            if (user.Roles is { } roles && roles.Any())
            {
                foreach (var role in roles)
                {
                    if (Permissions.RoleMap.ContainsKey(role))
                    {
                        permissions.UnionWith(Permissions.RoleMap[role]);
                    }
                }
            }

            return permissions;
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
                     e.AbsoluteExpiration = DateTime.Now.Add(_cacheExpiration);

                     return await GetAuthorizedOnMediaIdsInternalAsync(userId, cancellationToken);
                 });

            return ids;
        }

        public async Task<IEnumerable<Guid>> GetAuthorizedOnFaceIdsAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var cachekey = $"auth_on_face_{userId}";

            IEnumerable<Guid> ids = await _memoryCache.GetOrCreateAsync(
                cachekey,
                async (e) =>
                {
                    e.AbsoluteExpiration = DateTime.Now.Add(_cacheExpiration);
                    IEnumerable<Guid> mediaIds = await GetAuthorizedOnMediaIdsAsync(
                        userId,
                        cancellationToken);

                    return await _mediaStore.Faces.GetIdsByMediaAsync(mediaIds, cancellationToken);
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
                    e.AbsoluteExpiration = DateTime.Now.Add(_cacheExpiration);
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
                    e.AbsoluteExpiration = DateTime.Now.Add(_cacheExpiration);

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
