using MagicMedia.Identity.UI.Tests;
using MagicMedia.Identity.UI.Tests.Components;
using OpenQA.Selenium;

namespace Identity.UI.Tests.Components
{
    public class SignUpCompletedPage : Page
    {
        public SignUpCompletedPage()
        {
            SelfResolver = (driver)
                => driver.FindElementByTestId("page:success");
        }

        public IWebElement SuccessAlert =>
            Element.FindElement(By.CssSelector(".alert-success"));
    }
}
