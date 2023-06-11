using Microsoft.Extensions.Caching.Memory;

namespace MagicMedia.SamsungTv;

public class SamsungTvClientFactory : ISamsungTvClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly SamsungTvOptions _options;

    public SamsungTvClientFactory(
        IHttpClientFactory httpClientFactory,
        IMemoryCache memoryCache,
        SamsungTvOptions options)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
        _options = options;
    }

    public ISamsungTvArtModeClient Create(string name)
    {
        if (!_options.Devices.Any(x => x.Name.Equals(name)))
        {
            throw new ApplicationException($"No Samsung TV registered with name: {name}");
        }

        return new SamsungTvSamsungTvArtModeClient(name, _httpClientFactory, _memoryCache);
    }

    public IEnumerable<SamsungTvDevice> GetDevices()
    {
        return _options.Devices.ToList();
    }
}
