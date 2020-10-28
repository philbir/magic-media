using Xunit;


namespace MagicMedia.Api.Host.Tests.Infrastructure
{
    [CollectionDefinition(TestCollectionNames.ApiServer)]
    public class TestApiServerCollectionFixture : ICollectionFixture<ApiTestServer>
    {
    }

    [CollectionDefinition(TestCollectionNames.ApiServerWithIdentity)]
    public class TestApiServerWithIdentityCollectionFixture
        : ICollectionFixture<ApiTestServerWithIdentity>
    {
    }
}
