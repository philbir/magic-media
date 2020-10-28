using System;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.UI.Tests.Components;
using MagicMedia.Identity.UI.Tests;
using MagicMedia.Identity.UI.Tests.Components;
using OpenQA.Selenium.Support.Extensions;
using Xunit;

namespace Identity.UI.Tests
{
    [Collection(TestCollectionNames.Login)]
    public class LoginTests
    {
        private readonly IdentityTestContext _testContext;

        public LoginTests(IdentityTestContext testContext)
        {
            _testContext = testContext;
        }

        [Fact]
        public async Task Login_Diagnostics_SubjectValidated()
        {
            // Arrange
            await Task.Delay(TimeSpan.FromSeconds(5));
            using IdentityApp app = _testContext.CreateApp<IdentityApp>();

            // Act
            LoginPage page = app.Open<LoginPage>($"{app.TestContext.HostUrl}Diagnostics");
            page.Login("alice", "alice");

            DiagnosticsPage diag = app.WaitForPage<DiagnosticsPage>();

            var subject = diag.SubjectClaimValue.Text;

            // Assert
            subject.Should().Be("818727");
            OpenQA.Selenium.Screenshot ss = app.Driver.TakeScreenshot();
            ss.SaveAsFile(@$"c:\temp\magic-media.png");
        }
    }
}
