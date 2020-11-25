using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia
{
    public class MagicMediaServerBuilder : IMagicMediaServerBuilder
    {
        public MagicMediaServerBuilder(IConfiguration configuration, IServiceCollection services)
        {
            Configuration = configuration;
            Services = services;
        }

        public IConfiguration Configuration { get; }
        public IServiceCollection Services { get; }
    }

    public static class MagicMediaServerBuilderExtensions
    {
        public static IMagicMediaServerBuilder AddMagicMediaServer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var builder = new MagicMediaServerBuilder(configuration, services);
            builder.AddCoreMediaServices();

            return builder;
        }
    }
}
