using System;
using Serilog;
using UA = UAParser;

namespace MagicMedia.Thumbprint
{
    public class UserAgentInfoService : IUserAgentInfoService
    {
        private readonly UA.Parser _parser;

        public UserAgentInfoService()
        {
            _parser = UA.Parser.GetDefault();
        }

        public UserAgentInfo Parse(string userAgentString)
        {
            UA.ClientInfo? info = null;
            var ua = new UserAgentInfo(userAgentString);

            try
            {
                info = _parser.Parse(userAgentString);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error parsing userAgentString: {UserAgent}", userAgentString);
            }

            try
            {
                if (info != null)
                {
                    ua.OS = GetOS(info.OS);
                    ua.Agent = GetAgent(info.UA);
                    ua.Device = GetDevice(info.Device);
                    ua.IsRobot = info.UA.Family.Contains(
                        "Headless",
                        StringComparison.InvariantCultureIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error parsing userAgentString: {UserAgent}", userAgentString);
            }

            return ua;
        }

        private DeviceInfo? GetDevice(UA.Device? device)
        {
            var info = new DeviceInfo();

            if (device != null)
            {
                info.Brand = device.Brand;
                info.Model = device.Model;
                info.Family = device.Family;
            }

            return info;
        }

        private AgentInfo GetAgent(UA.UserAgent? ua)
        {
            var info = new AgentInfo();

            if (ua != null)
            {
                info.Family = ua.Family;
                info.Major = ua.Major;
                info.Minor = ua.Minor;
            }

            return info;
        }

        private OSInfo GetOS(UA.OS? os)
        {
            var osInfo = new OSInfo();

            if (os != null)
            {
                osInfo.Family = os.Family;
                osInfo.Major = os.Major;
                osInfo.Minor = os.Minor;
            }

            return osInfo;
        }
    }
}
