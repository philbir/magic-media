using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MagicMedia.Identity
{
    public static class WebHostEnvironmentExtensions
    {
        public static bool IsDemo(this IWebHostEnvironment hostEnvironment)
        {
            return hostEnvironment.IsEnvironment("Demo");
        }
    }
}
