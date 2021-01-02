using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Thumbprint
{
    [DebuggerDisplay("{ShortDescription}")]
    public class UserAgentShortInfo
    {
        public string ShortDescription { get; set; }

        public string Device { get; set; }

        public string OS { get; set; }

        public string Agent { get; set; }

        public bool IsRobot { get; set; }

        public override string ToString()
        {
            return ShortDescription;
        }
    }

    public class UserAgentInfo
    {
        public UserAgentInfo(string userAgentString)
        {
            UserAgentString = userAgentString;
        }

        public string UserAgentString { get; set; }

        public DeviceInfo? Device { get; set; }

        public OSInfo? OS { get; set; }

        public AgentInfo? Agent { get; set; }

        public bool IsRobot { get; set; }
    }

    public class DeviceInfo
    {
        public string? Brand { get; set; }

        public string? Model { get; set; }

        public string? Family { get; set; }
    }

    public class OSInfo
    {
        public string? Family { get; set; }

        public string? Major { get; set; }

        public string? Minor { get; set; }
    }

    public class AgentInfo
    {
        public string? Family { get; set; }

        public string? Major { get; set; }

        public string? Minor { get; set; }
    }
}
