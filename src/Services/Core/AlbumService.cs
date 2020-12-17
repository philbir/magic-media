using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Messaging;
using MagicMedia.Search;
using MagicMedia.Security;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia
{
    public class AlbumService : IAlbumService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IAlbumMediaIdResolver _mediaIdResolver;
        private readonly IUserContextFactory _userContextFactory;
        private readonly IBus _bus;

        public AlbumService(
            IMediaStore mediaStore,
            IAlbumMediaIdResolver mediaIdResolver,
            IUserContextFactory userContextFactory,
            IBus bus)
        {
            _mediaStore = mediaStore;
            _mediaIdResolver = mediaIdResolver;
            _userContextFactory = userContextFactory;
            _bus = bus;
        }

        public async Task<Album> AddAsync(string title, CancellationToken cancellationToken)
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = title
            };

            await _mediaStore.Albums.AddAsync(album, cancellationToken);

            return album;
        }

        public async Task<Album> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _mediaStore.Albums.GetByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Album>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _mediaStore.Albums.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<Album>> GetSharedByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.Albums.GetSharedByUserIdAsync(userId, cancellationToken);
        }

        public async Task<MediaThumbnail?> GetThumbnailAsync(
            Album album,
            ThumbnailSizeName size,
            CancellationToken cancellationToken)
        {
            Guid? mediaId;

            if (album.CoverMediaId.HasValue)
            {
                mediaId = album.CoverMediaId.Value;
            }
            else
            {
                mediaId = (await _mediaIdResolver.GetMediaIdsAsync(album, cancellationToken))
                    .FirstOrDefault();
            }

            if (mediaId.HasValue)
            {
                IReadOnlyDictionary<Guid, MediaThumbnail>? thumbs = await _mediaStore
                .GetThumbnailsByMediaIdsAsync(
                    new[] { mediaId.Value },
                    size,
                    cancellationToken);

                return thumbs.Values.FirstOrDefault();
            }

            return null;
        }

        public async Task<Album> AddItemsToAlbumAsync(
            AddItemToAlbumRequest request,
            CancellationToken cancellationToken)
        {
            Album? album;

            if (!request.AlbumId.HasValue)
            {
                album = await AddAsync(request.NewAlbumTitle!, cancellationToken);
            }
            else
            {
                album = await GetByIdAsync(request.AlbumId.Value, cancellationToken);
            }

            AddMediaIds(request, album);
            AddFolders(request, album);
            AddFilters(request, album);
                
            await _mediaStore.Albums.UpdateAsync(album, cancellationToken);

            await _bus.Publish(new ItemsAddedToAlbumMessage(album.Id));

            return album;
        }

        private void AddFilters(AddItemToAlbumRequest request, Album album)
        {
            if (request.Filters is { } filters && filters.Any())
            {
                AlbumInclude? include = album.Includes
                    .FirstOrDefault(x => x.Type == AlbumIncludeType.Query);

                if (include == null)
                {
                    include = new AlbumInclude() { Type = AlbumIncludeType.Query };
                    album.Includes.Add(include);
                }

                include.Filters = filters;
            }
        }

        public async Task<Album> RemoveFoldersAsync(RemoveFoldersFromAlbumRequest request, CancellationToken cancellationToken)
        {
            Album album = await GetByIdAsync(request.AlbumId, cancellationToken);

            foreach (AlbumInclude include in album.Includes.Where(x => x.Type == AlbumIncludeType.Folder))
            {
                var removed = include.Folders?.ToList();
                removed?.RemoveAll(f => request.Folders.Contains(f));

                include.Folders = removed;
            }

            await _mediaStore.Albums.UpdateAsync(album, cancellationToken);

            await _bus.Publish(new FoldersRemovedFromAlbum(album.Id, request.Folders));

            return album;
        }

        private static void AddMediaIds(AddItemToAlbumRequest request, Album album)
        {
            if (request.MediaIds is { } ids && ids.Any())
            {
                AlbumInclude? idInclude = album.Includes
                    .FirstOrDefault(x => x.Type == AlbumIncludeType.Ids);

                if (idInclude == null)
                {
                    idInclude = new AlbumInclude() { Type = AlbumIncludeType.Ids };
                    album.Includes.Add(idInclude);
                }
                var toAdd = new HashSet<Guid>(idInclude.MediaIds);

                foreach (Guid newId in ids)
                {
                    toAdd.Add(newId);
                }

                idInclude.MediaIds = toAdd;
            }
        }

        private static void AddFolders(AddItemToAlbumRequest request, Album album)
        {
            if (request.Folders is { } folders && folders.Any())
            {
                AlbumInclude? folderInclude = album.Includes
                    .FirstOrDefault(x => x.Type == AlbumIncludeType.Folder);

                if (folderInclude == null)
                {
                    folderInclude = new AlbumInclude() { Type = AlbumIncludeType.Folder };
                    album.Includes.Add(folderInclude);
                }

                var toAdd = new HashSet<string>(folderInclude.Folders);
                toAdd.AddRange(folders);

                folderInclude.Folders = toAdd;
            }
        }

        public async Task<SearchResult<Album>> SearchAsync(
            SearchAlbumRequest request,
            CancellationToken cancellationToken)
        {
            IUserContext userContext = await _userContextFactory.CreateAsync(cancellationToken);

            if (!userContext.HasPermission(Permissions.Album.ViewAll) && userContext.UserId.HasValue)
            {
                request.SharedWithUserId = userContext.UserId.Value;
            }

            return await _mediaStore.Albums.SearchAsync(request, cancellationToken);
        }

        public async Task<Album> UpdateAlbumAsync(
            UpdateAlbumRequest request,
            CancellationToken cancellationToken)
        {
            Album album = await GetByIdAsync(request.Id, cancellationToken);
            album.Title = request.Title;
            album.SharedWith = request.SharedWith;

            await _mediaStore.Albums.UpdateAsync(album, cancellationToken);

            return album;
        }
    }

    record AlbumData(
        IEnumerable<Media> Medias,
        IEnumerable<Person> Persons,
        IEnumerable<MediaFace> Faces);
}
