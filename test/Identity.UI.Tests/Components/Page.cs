using System;
using OpenQA.Selenium;

namespace MagicMedia.Identity.UI.Tests.Components
{
    public class Page
    {
        public IWebDriver Driver = null;

        protected IWebElement Element = null;

        protected Func<IWebDriver, IWebElement> SelfResolver = null;

        public Page()
        {
        }

        public Page(IWebDriver driver)
        {
            Driver = driver;
        }

        public void SetDriver(IWebDriver driver)
        {
            Driver = driver;
            if (SelfResolver != null)
            {
                Element = SelfResolver(Driver);
            }
        }
    }
}
