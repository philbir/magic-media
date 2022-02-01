using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace MagicMedia.Api.Security;

public class HttpContextClientInfoResolver : IUserClientInfoResolver
{
    private readonly HttpContext _httpContext;

    public HttpContextClientInfoResolver(HttpContext httpContext)
    {
        _httpContext = httpContext;
    }

    public ClientInfo Resolve()
    {
        return new ClientInfo
        {
            UserAgent = _httpContext.Request.Headers["User-Agent"],
            IPAdddress = _httpContext.GetRemoteIpAddress(),
            Request = $"{_httpContext.Request.Method}-{_httpContext.Request.GetDisplayUrl()}"
        };
    }
}
