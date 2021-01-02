using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Search;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;

namespace MagicMedia.Security
{
    public class UserService : IUserService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IMemoryCache _memoryCache;
        private readonly IBus _bus;
        private readonly IAlbumStore _albumStore;
        private readonly IAlbumMediaIdResolver _albumMediaIdResolver;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15);

        public UserService(
            IMediaStore mediaStore,
            IMemoryCache memoryCache,
            IBus bus,
            IAlbumStore albumStore,
            IAlbumMediaIdResolver albumMediaIdResolver)
        {
            _mediaStore = mediaStore;
            _memoryCache = memoryCache;
            _bus = bus;
            _albumStore = albumStore;
            _albumMediaIdResolver = albumMediaIdResolver;
        }

        public void InvalidateUserCacheAsync(Guid id)
        {
            string[] keys = new[] { "mm_user", "auth_on_media", "auth_on_face", "auth_on_album", "auth_on_person" };

            foreach ( var key in keys)
            {
                _memoryCache.Remove($"{key}_{id}");
            }
        }

        public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await GetByIdAsync(id, bypassCache: false, cancellationToken);
        }

        public async Task<User> GetByIdAsync(Guid id, bool bypassCache, CancellationToken cancellationToken)
        {
            if (bypassCache)
            {
                return await _mediaStore.Users.TryGetByIdAsync(id, cancellationToken);
            }

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

        public async Task<SearchResult<User>> SearchAsync(
            SearchUserRequest request,
            CancellationToken cancellationToken)
        {

            return await _mediaStore.Users.SearchAsync(request, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _mediaStore.Users.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetManyAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.Users.GetManyAsync(ids, cancellationToken);
        }

        public async Task<User> TryGetByPersonIdAsync(
            Guid personId,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.Users.TryGetByPersonIdAsync(personId, cancellationToken);
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

        public async Task<User> CreateInviteAsync(Guid userId, CancellationToken cancellationToken)
        {
            User user = await GetByIdAsync(userId, cancellationToken);

            var message = new InviteUserRequestedMessage(user.Id, user.Name, user.Email);

            await _bus.Publish(message);

            return user;
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

        public Task<User> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            return _mediaStore.Users.UpdateAsync(user, cancellationToken);
        }
    }
}
