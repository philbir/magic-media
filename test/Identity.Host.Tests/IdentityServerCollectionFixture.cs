using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MagicMedia.Identity.Host.Tests
{
    [CollectionDefinition(TestCollectionNames.HostIdentityServer)]
    public class IdentityServerCollectionFixture : ICollectionFixture<IdentityTestServer>
    {
    }
}
