using OpenQA.Selenium;

namespace MagicMedia.Identity.UI.Tests.Components
{
    public class SignUpPage : Page
    {
        public SignUpPage()
        {
            SelfResolver = (driver)
                => driver.FindElementByTestId("page:signup");
        }

        public IWebElement Email =>
            Element.FindElement(By.Id("Email"));

        public IWebElement Mobile =>
            Element.FindElement(By.Id("Mobile"));

        public IWebElement SubmitButton =>
            Element.FindElement(By.CssSelector("button[value='signup']"));

        public void SignUp(string email, string mobile)
        {
            Email.SendKeys(email);
            Mobile.SendKeys(mobile);

            SubmitButton.Click();
        }
    }


    public class LoginPage : Page
    {
        public LoginPage()
        {
            SelfResolver = (driver)
                => driver.FindElementByTestId("page:login");
        }


        public IWebElement Username =>
            Element.FindElement(By.Id("Username"));

        public IWebElement Password =>
            Element.FindElement(By.Id("Password"));

        public IWebElement SubmitButton =>
            Element.FindElement(By.CssSelector("button[value='login']"));

        public void Login(string username, string password)
        {
            Username.SendKeys(username);
            Password.SendKeys(password);

            SubmitButton.Click();
        }
    }
}
