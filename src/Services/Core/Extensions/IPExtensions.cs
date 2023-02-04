using System.Net;

namespace MagicMedia.Extensions;

public static class IPExtensions
{
    public static bool IsInternalIP(this string ipAddress)
    {
        if (ipAddress == "::1" || ipAddress is null)
            return true;

        byte[] ip = IPAddress.Parse(ipAddress).GetAddressBytes();
        switch (ip[0])
        {
            case 10:
            case 127:
                return true;
            case 172:
                return ip[1] >= 16 && ip[1] < 32;
            case 192:
                return ip[1] == 168;
            default:
                return false;
        }
    }
}
