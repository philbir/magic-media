using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace MagicMedia.Identity.UI.Tests
{
    public class WebDriverFactory
    {
        public static IWebDriver Build(
            SeleniumDriverMode mode,
            string browser, string
            remoteUrl = null)
        {
            IWebDriver driver;

            if (mode == SeleniumDriverMode.Local)
            {
                switch (browser)
                {
                    case "Edge_Local":
                        driver = CreateEdgeLocal();
                        break;
                    case "FireFox_Local":
                        driver = CreateFirefoxLocal();
                        break;
                    default:
                        driver = CreateChromeLocal();
                        break;
                }
            }
            else
            {
                driver = CreateRemote(remoteUrl);
            }

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            return driver;
        }

        private static IWebDriver CreateChromeLocal()
        {
            var options = new ChromeOptions();
            //options.AddArgument("--start-maximized");
            options.AddArgument("--window-size=1400,1080");
            options.AddArgument("--allow-insecure-localhost");
            options.AddUserProfilePreference("download.default_directory",
                                             Path.GetTempPath());

            return new ChromeDriver("./", options);
        }

        private static IWebDriver CreateFirefoxLocal()
        {
            return new FirefoxDriver("./");
        }

        private static IWebDriver CreateEdgeLocal()
        {
            return new EdgeDriver("./");
        }

        private static IWebDriver CreateRemote(string baseUrl)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--whitelisted-ips");
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-extensions");

            FirefoxOptions ff = new FirefoxOptions();
            ff.AddArgument("--headless");

            IWebDriver driver = new RemoteWebDriver(
                  new Uri($"{baseUrl}wd/hub"), ff.ToCapabilities());

            return driver;
        }
    }
}
