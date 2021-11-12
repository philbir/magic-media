namespace MagicMedia.Thumbprint
{
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
}
