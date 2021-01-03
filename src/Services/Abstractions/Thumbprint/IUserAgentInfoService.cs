namespace MagicMedia.Thumbprint
{
    public interface IUserAgentInfoService
    {
        UserAgentInfo Parse(string userAgentString);

        UserAgentShortInfo GetShortInfo(UserAgentInfo agentInfo);
    }
}
