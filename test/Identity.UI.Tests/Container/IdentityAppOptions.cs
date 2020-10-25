using Squadron;

namespace Identity.UI.Tests.Container
{
    public class IdentityAppOptions : ComposeResourceOptions
    {
        public override void Configure(ComposeResourceBuilder builder)
        {
            builder.AddContainer<IdentityDbOptions>("mongo");
            builder.AddContainer<SelemiumFirefoxServerOptions>("selenium");
            builder.AddContainer<IdentityHostOptions>("host")
                    .AddLink("mongo",
                             new EnvironmentVariableMapping(
                                "Identity:Database:ConnectionString",
                                "#CONNECTIONSTRING_INTERNAL#"));
        }
    }
}
