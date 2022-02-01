using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia;

public interface IMagicMediaServerBuilder
{
    IConfiguration Configuration { get; }
    IServiceCollection Services { get; }
}
