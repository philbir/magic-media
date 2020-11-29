using OpenQA.Selenium;

namespace MagicMedia.Identity.UI.Tests
{
    public static class WebElementExtensions
    {
        public static IWebElement FindElementByTestId(this ISearchContext element, string testId)
        {
            return element.FindElement(By.CssSelector($"[data-test-id='{testId}']"));
        }
    }
}
