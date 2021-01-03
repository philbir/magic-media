using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Thumbprint
{
    public static class UserAgentInfoExtensions
    {
        public static string? ToShortDescription(this DeviceInfo? deviceInfo)
        {
            if (deviceInfo == null)
                return null;

            var data = new List<string>();
            data.AddInfo(deviceInfo.Brand);
            data.AddInfo(deviceInfo.Model);

            return string.Join(" ", data);
        }

        public static string? ToShortDescription(this UserAgentInfo? userAgent)
        {
            if (userAgent == null)
                return null;

            var data = new List<string>();
            data.AddInfo(userAgent.Device?.ToShortDescription());
            data.AddInfo(userAgent.OS?.ToShortDescription());
            data.AddInfo(userAgent.Agent?.ToShortDescription());

            return string.Join(" ", data);
        }

        public static string? ToShortDescription(this OSInfo? osInfo)
        {
            if (osInfo == null)
                return null;

            var data = new List<string>();
            data.AddInfo(osInfo.Family);
            data.AddInfo(osInfo.Major);

            return string.Join(" ", data);
        }

        public static string? ToShortDescription(this AgentInfo? agentInfo)
        {
            if (agentInfo == null)
                return null;

            var data = new List<string>();
            data.AddInfo(agentInfo.Family);
            data.AddInfo(agentInfo.Major);

            return string.Join(" ", data);
        }

        private static void AddInfo(this IList<string> data, string? value)
        {
            if (!string.IsNullOrEmpty(value) &&
                !value.Equals("other", StringComparison.OrdinalIgnoreCase))
            {
                data.Add(value);
            }
        }
    }
}
