using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StrawberryShake;
using StrawberryShake.Http;
using StrawberryShake.Http.Pipelines;
using StrawberryShake.Serializers;

namespace MagicMedia.Api.Host.Tests
{
    public static class MagicMediaTestServiceCollectionExtensions
    {
        private const string _clientName = "MagicMediaTest";

        public static IServiceCollection AddMagicMediaTest(
            this IServiceCollection serviceCollection)
        {
            if (serviceCollection is null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.AddSingleton<IMagicMediaTest, MagicMediaTest>();
            serviceCollection.AddSingleton<IOperationExecutorFactory>(sp =>
                new HttpOperationExecutorFactory(
                    _clientName,
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient,
                    PipelineFactory(sp),
                    sp));

            serviceCollection.TryAddSingleton<IOperationExecutorPool, OperationExecutorPool>();

            serviceCollection.AddDefaultScalarSerializers();
            serviceCollection.AddEnumSerializers();
            serviceCollection.AddResultParsers();

            serviceCollection.TryAddDefaultOperationSerializer();
            serviceCollection.TryAddDefaultHttpPipeline();

            return serviceCollection;
        }

        private static IServiceCollection AddDefaultScalarSerializers(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IValueSerializerResolver, ValueSerializerResolver>();

            foreach (IValueSerializer serializer in ValueSerializers.All)
            {
                serviceCollection.AddSingleton(serializer);
            }

            return serviceCollection;
        }

        private static IServiceCollection AddEnumSerializers(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IValueSerializer, ThumbnailSizeNameValueSerializer>();
            return serviceCollection;
        }


        private static IServiceCollection AddResultParsers(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IResultParserResolver, ResultParserResolver>();
            serviceCollection.AddSingleton<IResultParser, MediaDetailsResultParser>();
            return serviceCollection;
        }

        private static IServiceCollection TryAddDefaultOperationSerializer(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IOperationFormatter, JsonOperationFormatter>();
            return serviceCollection;
        }

        private static IServiceCollection TryAddDefaultHttpPipeline(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<OperationDelegate>(
                sp => HttpPipelineBuilder.New()
                    .Use<CreateStandardRequestMiddleware>()
                    .Use<SendHttpRequestMiddleware>()
                    .Use<ParseSingleResultMiddleware>()
                    .Build(sp));
            return serviceCollection;
        }

        private static OperationDelegate PipelineFactory(IServiceProvider services)
        {
            return services.GetRequiredService<OperationDelegate>();
        }
    }
}
