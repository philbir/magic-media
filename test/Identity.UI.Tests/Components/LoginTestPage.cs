using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityModel.Client;
using MagicMedia.Identity.UI.Tests.Extensions;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace MagicMedia.Identity.UI.Tests.Components
{
    public class LoginTestPage : Page
    {
        public LoginTestPage()
        {
            SelfResolver = (driver)
                => driver.FindElementByTestId("page:authorize");
        }

        public LoginTestPage(IWebDriver driver) : base(driver)
        {
        }

        public IWebElement LoginButton
            => Element.FindElementByTestId("btn:login");

        public IWebElement Form
            => Element.FindElement(By.TagName("form"));

        public IWebElement Authority
            => Form.FindElement(By.CssSelector("input[name='authority']"));

        public IWebElement UserData
             => Driver.FindElement(By.Id("user_data"));

        public void SelectEnvironment(string env)
        {
            IWebElement btn = Element.FindElementByTestId("btn:favorites");
            btn.Click();
            Thread.Sleep(500);
            IWebElement favItem = Driver.FindElementByTestId($"{env.ToUpper()}_OneLogin_SPA01");
            Thread.Sleep(500);
            favItem.Click();
        }

        public async Task<TestClientUserData> GetUserClaimsAsync()
        {
            var json = UserData.GetAttribute("value");

            if (json != null)
                return await TestClientUserData.CreateFromJsonAsync(json);
            return null;
        }
    }

    public class TestClientUserData
    {
        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("session_state")]
        public string SessionState { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        public IEnumerable<Claim> IdTokenClaims { get; private set; }
        public IEnumerable<Claim> AccessTokenClaims { get; private set; }
        public IEnumerable<Claim> UserInfoClaims { get; private set; }


        public void ReadTokens()
        {
            IdTokenClaims = IdToken.GetClaims();
            AccessTokenClaims = AccessToken.GetClaims();
        }



        public static async Task<TestClientUserData> CreateFromJsonAsync(string json)
        {
            TestClientUserData userData = JsonConvert.DeserializeObject<TestClientUserData>(json);
            userData.ReadTokens();
            userData.UserInfoClaims = await userData.LoadUserInfoClaimsAsync();
            return userData;
        }

        private async Task<IEnumerable<Claim>> LoadUserInfoClaimsAsync()
        {
            var iss = AccessTokenClaims.First(x => x.Type == JwtClaimTypes.Issuer).Value;
            var httpClient = new HttpClient();

            UserInfoResponse userInfoRes = await httpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = $"{iss}/connect/userinfo",
                Token = AccessToken
            });

            return userInfoRes.Claims;
        }

        public void AssertUserInfoClaims(IEnumerable<Claim> assertion)
        {
            AssertClaims(UserInfoClaims, assertion);
        }

        private void AssertClaims(IEnumerable<Claim> claims, IEnumerable<Claim> assertion)
        {
            foreach (Claim assert in assertion)
            {
                bool hasClaim = claims.FirstOrDefault(
                    x => x.Type == assert.Type &&
                    x.Value == assert.Value) != null;

                hasClaim.Should()
                    .BeTrue($"Expected claim with type: {assert.Type} " +
                            $"and value: {assert.Value}");
            }
        }
    }
}
