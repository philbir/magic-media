using OpenQA.Selenium;

namespace MagicMedia.Identity.UI.Tests.Components
{
    public class ValidateMobilePage : Page
    {
        public ValidateMobilePage()
        {
            SelfResolver = (driver)
                => driver.FindElementByTestId("page:validate");
        }

        public IWebElement Code =>
            Element.FindElement(By.Id("Code"));

        public IWebElement SubmitButton =>
            Element.FindElement(By.CssSelector("button[value='validate']"));

        public void EnterCode(string code)
        {
            Code.SendKeys(code);

            SubmitButton.Click();
        }
    }
}
