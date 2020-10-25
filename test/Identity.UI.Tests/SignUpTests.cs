using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Identity.UI.Tests;
using MagicMedia.Identity.UI.Tests.Components;
using Magnet.Client;
using Xunit;

namespace Identity.UI.Tests
{
    [Collection(TestCollectionNames.Login)]
    public class SignUpTests
    {
        private readonly IdentityTestContext _testContext;

        public SignUpTests(IdentityTestContext testContext)
        {
            _testContext = testContext;
        }

        [Fact]
        public async Task SignUp_Created()
        {
            // Arrange
            using IdentityApp app = _testContext.CreateApp<IdentityApp>();
            await app.TestContext.StartMagnetAsync();
            string mobileNr = "+41798074288";

            // Act
            SignUpPage page = app.Open<SignUpPage>($"{app.TestContext.HostUrl}SignUp");

            page.SignUp("test1@magic-media.io", mobileNr);

            SmsMessage sms = await app.TestContext.MagnetSession.WaitForSms(mobileNr);

            // Assert

        }
    }
}
