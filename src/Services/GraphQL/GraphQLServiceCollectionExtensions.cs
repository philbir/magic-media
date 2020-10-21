using HotChocolate.Execution.Configuration;
using MagicMedia.GraphQL;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.GraphQL.Face;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia
{
    public static class GrapQLServiceCollectionExtensions
    {
        public static IRequestExecutorBuilder AddMagicMediaGrapQL(
            this IRequestExecutorBuilder builder)
        {
            builder
                .AddQueryType(d => d.Name("Query"))
                .AddType<MediaQueries>()
                .AddType<FaceQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                .AddType<FaceMutations>()
                .AddType<MediaType>()
                .AddType<FaceType>()
                .AddType<ThumbnailType>()
                .AddDataLoader<CameraByIdDataLoader>()
                .AddDataLoader<ThumbnailByMediaIdDataLoader>();

            return builder;
        }
    }
}
