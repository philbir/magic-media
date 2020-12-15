using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using MagicMedia.GraphQL;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.GraphQL.Face;
using MagicMedia.GraphQL.SearchFacets;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia
{
    public static class GrapQLServiceCollectionExtensions
    {
        public static IMagicMediaServerBuilder AddGraphQLServer(
            this IMagicMediaServerBuilder builder)
        {
            builder.Services.AddGraphQLServer()
                .AddMagicMediaGrapQL();

            return builder;
        }

        private static IRequestExecutorBuilder AddMagicMediaGrapQL(
            this IRequestExecutorBuilder builder)
        {
            builder
                .AddQueryType(d => d.Name("Query"))
                .AddType<MediaQueries>()
                .AddType<FaceQueries>()
                .AddType<PersonQueries>()
                .AddType<UserQueries>()
                .AddType<SearchFacetQueries>()
                .AddType<AlbumQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                .AddType<FaceMutations>()
                .AddType<MediaMutations>()
                .AddType<PersonMutations>()
                .AddType<AlbumMutations>()
                .AddType<UserMutations>()
                .AddType<MediaType>()
                .AddType<VideoInfoType>()
                .AddType<FaceType>()
                .AddType<PersonType>()
                .AddType<UserType>()
                .AddType<AlbumType>()
                .AddType<ThumbnailType>()
                .AddType<SearchFacetType>()
                .RenameRequestToInput<RemoveFoldersFromAlbumRequest>()
                .RenameRequestToInput<CreateUserFromPersonRequest>()
                .AddDataLoader<CameraByIdDataLoader>()
                .AddDataLoader<ThumbnailByMediaIdDataLoader>()
                .AddDataLoader<MediaByIdDataLoader>()
                .AddDataLoader<ThumbnailDataDataLoader>()
                .AddInMemorySubscriptions()
                .AddAuthorization();

            return builder;
        }


        private static IRequestExecutorBuilder RenameRequestToInput<T>(
            this IRequestExecutorBuilder builder)
        {
            var name = typeof(T).Name.Replace("Request", "Input");
            builder.AddInputObjectType<T>(d => d.Name(name));

            return builder;
        }
    }
}
