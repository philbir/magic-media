using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.UI.Tests.Components;
using MagicMedia.Identity.Data;
using MagicMedia.Identity.UI.Tests;
using MagicMedia.Identity.UI.Tests.Components;
using Magnet.Client;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
        public async Task SignUp_SessionCrated()
        {
            // Arrange
            await Task.Delay(TimeSpan.FromSeconds(5));
            using IdentityApp app = _testContext.CreateApp<IdentityApp>();
            await app.TestContext.StartMagnetAsync();
            string mobileNr = "+41798074288";
            string email = $"{Guid.NewGuid().ToString("N").Substring(0, 6)}@magic-media.io";

            // Act
            SignUpPage page = app.Open<SignUpPage>($"{app.TestContext.HostUrl}SignUp");
            page.SignUp(email, mobileNr);

            ValidateMobilePage validatePage = app.WaitForPage<ValidateMobilePage>();

            SmsMessage sms = await app.TestContext.MagnetSession.WaitForSms(mobileNr);

            var code = ParseSecurityCode(sms.Body);
            validatePage.EnterCode(code);

            SignUpCompletedPage completed = app.WaitForPage<SignUpCompletedPage>();

            // Assert
            completed.SuccessAlert.Text.Should().Be("Registration completed");

            SignUpSession session = await GetSessionAsync(email);
            session.State.Should().Be("Completed");
        }

        private async Task<SignUpSession> GetSessionAsync(string email)
        {
            IIdentityDbContext db = _testContext.GetDbContext();

            SignUpSession session = await db.SignUpSessions.AsQueryable()
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();

            return session;
        }

        public string ParseSecurityCode(string body)
        {
            var regex = new Regex("([0-9{6}])");
            var code = string.Join("", regex.Matches(body));
            return code;
        }
    }
}
