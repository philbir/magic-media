using HotChocolate.Execution.Configuration;
using MagicMedia.Authorization;
using MagicMedia.GraphQL;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.GraphQL.Face;
using MagicMedia.GraphQL.SearchFacets;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia;

public static class GrapQLServiceCollectionExtensions
{
    public static IMagicMediaServerBuilder AddGraphQLServer(
        this IMagicMediaServerBuilder builder)
    {
        builder.Services.AddGraphQLServer()
            .AddMagicMediaGrapQL();

        //builder.Services.AddHttpResultSerializer<ForbiddenHttpResultSerializer>();

        return builder;
    }

    private static IRequestExecutorBuilder AddMagicMediaGrapQL(
        this IRequestExecutorBuilder builder)
    {
        builder
            .AddQueryType(d => d.Name(RootTypes.Query))
                //.Authorize(AuthorizationPolicies.Names.ApiAccess))
            .AddTypeExtension<MediaExportProfileQueries>()
            .AddTypeExtension<MediaQueries>()
            .AddTypeExtension<FaceQueries>()
            .AddTypeExtension<PersonQueries>()
            .AddTypeExtension<UserQueries>()
            .AddTypeExtension<SearchFacetQueries>()
            .AddTypeExtension<AlbumQueries>()
            .AddTypeExtension<CameraQueries>()
            .AddTypeExtension<AuditQueries>()
            .AddMutationType(d => d.Name(RootTypes.Mutation)
                .Authorize(AuthorizationPolicies.Names.ApiAccess))
            .AddTypeExtension<FaceMutations>()
            .AddTypeExtension<MediaMutations>()
            .AddTypeExtension<PersonMutations>()
            .AddTypeExtension<AlbumMutations>()
            .AddTypeExtension<UserMutations>()
            .AddType<MediaType>()

            .AddType<VideoInfoType>()
            .AddType<FaceType>()
            .AddType<PersonType>()
            .AddType<SimilarMediaGroupType>()
            .AddType<UserType>()
            .AddType<AlbumType>()
            .AddType<ThumbnailType>()
            .AddType<SearchFacetType>()
            .AddType<AuditEventType>()
            .AddType<UserAgentInfoType>()
            .AddType<MediaExportProfileType>()
            .AddType(new UuidType(defaultFormat: 'N'))
            .RenameRequestToInput<RemoveFoldersFromAlbumRequest>()
            .RenameRequestToInput<CreateUserFromPersonRequest>()
            .RenameRequestToInput<SearchUserRequest>()
            .RenameRequestToInput<SaveUserSharedAlbumsRequest>()
            .RenameRequestToInput<SearchAuditRequest>()
            .RenameRequestToInput<SetAlbumCoverRequest>()
            .AddDataLoader<CameraByIdDataLoader>()
            .AddDataLoader<ThumbnailByMediaIdDataLoader>()
            .AddDataLoader<MediaByIdDataLoader>()
            .AddDataLoader<ThumbnailDataDataLoader>()
            .AddDataLoader<UserByIdDataLoader>()
            .AddDataLoader<ClientThumbprintByIdDataLoader>()
            .AddInMemorySubscriptions()
            .AddInstrumentation(o =>
            {
                o.RenameRootActivity = true;
            })
            .AddMutationConventions()
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
