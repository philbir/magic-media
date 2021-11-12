using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class AlbumType : ObjectType<Album>
    {
        protected override void Configure(IObjectTypeDescriptor<Album> descriptor)
        {
            descriptor
                .Field("thumbnail")
                .Argument("size", a => a
                    .DefaultValue(ThumbnailSizeName.M)
                    .Type(typeof(ThumbnailSizeName)))
                .ResolveWith<Resolvers>(_ => _.GetThumbnailAsync(default!, default!, default!, default!));

            descriptor
                .Field("folders")
                .ResolveWith<Resolvers>(_ => _.GetFolders(default!));

            descriptor
                .Field("filters")
                .ResolveWith<Resolvers>(_ => _.GetFilters(default!));

            descriptor
                .Field("allMediaIds")
                .ResolveWith<Resolvers>(_ => _.GetAllMediaIdsAsync(default!, default!, default!));
        }

        class Resolvers
        {
            public Task<MediaThumbnail?> GetThumbnailAsync(
                [Parent] Album album,
                [Service] IAlbumService albumService,
                ThumbnailSizeName size,
                CancellationToken cancellationToken)
                    => albumService.GetThumbnailAsync(album, size, cancellationToken);


            public IEnumerable<string>? GetFolders(
                [Parent] Album album)
            {
                return album.Includes?
                    .FirstOrDefault(x => x.Type == AlbumIncludeType.Folder)?
                    .Folders;
            }

            public IEnumerable<FilterDescription>? GetFilters(Album album)
            {
                return album.Includes?
                    .FirstOrDefault(x => x.Type == AlbumIncludeType.Query)?
                    .Filters;
            }

            public Task<IEnumerable<Guid>?> GetAllMediaIdsAsync(
                Album album,
                [Service] IAlbumMediaIdResolver albumMediaIdResolver,
                CancellationToken cancellationToken)
                    =>  albumMediaIdResolver.GetMediaIdsAsync(album, cancellationToken);
        }
    }
}
