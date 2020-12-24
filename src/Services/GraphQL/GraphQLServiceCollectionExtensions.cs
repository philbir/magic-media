using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using MagicMedia.Authorization;
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

            builder.Services.AddHttpResultSerializer<ForbiddenHttpResultSerializer>();

            return builder;
        }

        private static IRequestExecutorBuilder AddMagicMediaGrapQL(
            this IRequestExecutorBuilder builder)
        {
            builder
                .AddQueryType(d => d.Name("Query")
                    .Authorize(AuthorizationPolicies.Names.ApiAccess))
                .AddType<MediaQueries>()
                .AddType<FaceQueries>()
                .AddType<PersonQueries>()
                .AddType<UserQueries>()
                .AddType<SearchFacetQueries>()
                .AddType<AlbumQueries>()
                .AddType<CameraQueries>()
                .AddMutationType(d => d.Name("Mutation")
                    .Authorize(AuthorizationPolicies.Names.ApiAccess))
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
                .RenameRequestToInput<SearchUserRequest>()
                .RenameRequestToInput<SaveUserSharedAlbumsRequest>()
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
