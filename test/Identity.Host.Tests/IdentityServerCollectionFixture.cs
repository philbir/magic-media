using Xunit;

namespace MagicMedia.Identity.Host.Tests
{
    [CollectionDefinition(TestCollectionNames.HostIdentityServer)]
    public class IdentityServerCollectionFixture : ICollectionFixture<IdentityTestServer>
    {
    }
}
