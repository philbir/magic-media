using MagicMedia.Identity.UI.Tests;
using Microsoft.Extensions.Configuration;
using Squadron;

namespace Identity.UI.Tests.Container
{
    public class IdentityHostOptions : GenericContainerOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            IConfiguration config = IdentityTestContext.BuildConfiguration();
            string passwordKey = "ECall:Password";
            string fromKey = "ECall:From";

            base.Configure(builder);
            builder
                .Name("identity")
                .WaitTimeout(30)
                .PreferLocalImage()
                .InternalPort(80)
                .ExternalPort(80)
                .AddNetwork("identity-net")
                .AddEnvironmentVariable(
                    $"{passwordKey}={config.GetValue<string>(passwordKey)}")
                .AddEnvironmentVariable(
                    $"{fromKey}={config.GetValue<string>(fromKey)}")
                .Image("magic-media-identity:dev");
        }
    }
}
