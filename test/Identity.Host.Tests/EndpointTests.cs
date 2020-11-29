using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Snapshooter.Xunit;
using Xunit;

namespace MagicMedia.Identity.Host.Tests
{
    [Collection(TestCollectionNames.HostIdentityServer)]
    public class EndpointTests
    {
        private readonly IdentityTestServer _testServer;

        public EndpointTests(IdentityTestServer testServer)
        {
            _testServer = testServer;
        }

        [Fact]
        public async Task TokenEndpoint_ClientCredentials_Valid_TokenIssued()
        {
            //Arrange
            var tokenRequest = new ClientCredentialsTokenRequest
            {
                ClientId = "Test.CC01",
                ClientSecret = "secret",
                Scope = "api.magic.read",
                Address = _testServer.HttpClient!.BaseAddress + "connect/token"
            };

            //Act
            TokenResponse tokenResponse = await _testServer.HttpClient
                .RequestClientCredentialsTokenAsync(tokenRequest);

            //Assert
            tokenResponse.HttpStatusCode.Should().Be(HttpStatusCode.OK);

            var anaylzer = new JwtTokenAnalyzer(tokenResponse.AccessToken);
            anaylzer.AnalyzableClaims.MatchSnapshot();
        }
    }
}
