using System;
using OpenQA.Selenium;
using MagicMedia.Identity.UI.Tests.Components;

namespace MagicMedia.Identity.UI.Tests
{
    public class IdentityApp : IDisposable
    {
        public IWebDriver Driver { get; internal set; }

        public IdentityTestContext TestContext { get; internal set; }

        public TPage Open<TPage>(string url)
            where TPage : Page, new()
        {
            Driver.Navigate()
                .GoToUrl(url);

            return WaitForPage<TPage>();
        }

        public TPage WaitForPage<TPage>() where TPage : Page, new()
        {
            var page = new TPage();
            page.SetDriver(Driver);
            return page;
        }

        protected virtual void Dispose(bool disposing)
        {
            //if (Driver != null)
            //    Driver.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
