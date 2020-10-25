using Xunit;

namespace MagicMedia.Identity.UI.Tests
{
    [CollectionDefinition(TestCollectionNames.Login)]
    public class IdentityServerCollectionFixture : ICollectionFixture<IdentityTestContext>
    {
    }
}
