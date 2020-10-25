using System;
using System.Collections.Generic;
using System.Text;
using MagicMedia.Identity.UI.Tests;
using MagicMedia.Identity.UI.Tests.Components;
using OpenQA.Selenium;

namespace Identity.UI.Tests.Components
{
    public class DiagnosticsPage : Page
    {
        public DiagnosticsPage()
        {
            SelfResolver = (driver)
                => driver.FindElementByTestId("page:diagnostic");
        }

        public IWebElement SubjectClaimValue =>
            Element.FindElementByTestId("claimvalue:sub");
    }
}
