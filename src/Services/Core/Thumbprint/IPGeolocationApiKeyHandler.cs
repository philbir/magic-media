using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Thumbprint;

public class IPGeolocationApiKeyHandler : DelegatingHandler
{
    private readonly IPGeolocationApiOptions _options;

    public IPGeolocationApiKeyHandler(IPGeolocationApiOptions options)
    {
        _options = options;
    }


    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var uriBuilder = new UriBuilder(request.RequestUri!);
        uriBuilder.Query = $"{uriBuilder.Query}&apiKey={_options.ApiKey}";
        request.RequestUri = uriBuilder.Uri;

        return base.SendAsync(request, cancellationToken);
    }
}
