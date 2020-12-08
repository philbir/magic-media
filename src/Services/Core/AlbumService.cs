using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Messaging;
using MagicMedia.Search;
using MagicMedia.Store;
using MassTransit;
using SixLabors.ImageSharp.ColorSpaces;

namespace MagicMedia
{
    public class AlbumService : IAlbumService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IBus _bus;

        public AlbumService(
            IMediaStore mediaStore,
            IBus bus)
        {
            _mediaStore = mediaStore;
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
                mediaId = (await GetMediaIdsAsync(album, cancellationToken))
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

            await _mediaStore.Albums.UpdateAsync(album, cancellationToken);

            await _bus.Publish(new ItemsAddedToAlbumMessage(album.Id));

            return album;
        }

        public async Task<Album> RemoveFoldersAsync(RemoveFoldersFromAlbumRequest request, CancellationToken cancellationToken)
        {
            Album album = await GetByIdAsync(request.AlbumId, cancellationToken);

            foreach (AlbumInclude include in album.Includes.Where(x => x.Type == AlbumIncludeType.Folder))
            {
                include.Folders?.RemoveAll(f => request.Folders.Contains(f));
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

                foreach (Guid newId in request.MediaIds)
                {
                    toAdd.Add(newId);
                }

                idInclude.MediaIds = toAdd;
            }
        }

        public async Task<SearchResult<Album>> SearchAsync(
            SearchAlbumRequest request,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.Albums.SearchAsync(request, cancellationToken);
        }

        public async Task<IEnumerable<Guid>> GetMediaIdsAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            Album? album = await GetByIdAsync(id, cancellationToken);

            return await GetMediaIdsAsync(album, cancellationToken);
        }

        public async Task<IEnumerable<Guid>> GetMediaIdsAsync(
            Album album,
            CancellationToken cancellationToken)
        {
            HashSet<Guid> ids = new();

            foreach (AlbumInclude include in album.Includes)
            {
                switch (include.Type)
                {
                    case AlbumIncludeType.Ids:
                        ids.AddRange(include.MediaIds);
                        break;
                    case AlbumIncludeType.Folder:

                        foreach (var folder in include.Folders!)
                        {
                            IEnumerable<Guid> folderIds = await _mediaStore.GetIdsByFolderAsync(folder, cancellationToken);
                            ids.AddRange(folderIds);
                        }
                        break;
                }
            }

            return ids;
        }

        public async Task<Album> UpdateAlbumAsync(
            UpdateAlbumRequest request,
            CancellationToken cancellationToken)
        {
            Album album = await GetByIdAsync(request.Id, cancellationToken);
            album.Title = request.Title;

            await _mediaStore.Albums.UpdateAsync(album, cancellationToken);

            return album;
        }
    }

    record AlbumData(
        IEnumerable<Media> Medias,
        IEnumerable<Person> Persons,
        IEnumerable<MediaFace> Faces);
}
