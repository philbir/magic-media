using System;
using System.Collections.Generic;
using System.Text;

namespace MagicMedia.Identity.UI.Tests
{
    public class IdentityTestOptions
    {
        public DriverOptions Driver { get; set; }

        public MagnetOptions Magnet { get; set; }
    }

    public class MagnetOptions
    {
        public string Url { get; set; }

        public string Name { get; set; }
    }

    public class DriverOptions
    {
        public SeleniumDriverMode Mode { get; set; }

        public string Browser { get; set; }
    }

    public enum SeleniumDriverMode
    {
        Local,
        Remote
    }
}
