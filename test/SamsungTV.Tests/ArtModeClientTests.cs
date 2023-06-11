using MagicMedia.SamsungTv;
using MagicMedia.TestLibrary;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.SamsungTV.Tests;

public class ArtModeClientTests
{
    [Fact]
    public async Task GetAllImages()
    {
        SamsungTvClientFactory factory = CreateFactory();
        ISamsungTvArtModeClient client = factory.Create("Tv");

        await client.GetAllMediaAsync(CancellationToken.None);
    }

    [Fact]
    public async Task IsOnline()
    {
        SamsungTvClientFactory factory = CreateFactory();

        ISamsungTvArtModeClient client = factory.Create("Tv");

        var isOnline = await client.IsOnlineAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Upload()
    {
        SamsungTvClientFactory factory = CreateFactory();

        ISamsungTvArtModeClient client = factory.Create("Tv");

        var image = GetBytes(TestMediaLibrary.SamsungFrameFormat);

        await client.UploadAsync(image, "Test.jpg", null, CancellationToken.None);
    }

    private byte[] GetBytes(Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    private static SamsungTvClientFactory CreateFactory()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddHttpClient("Tv")
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:8085"));

        var options = new SamsungTvOptions();
        options.Devices = new[] { new SamsungTvDevice { Name = "Tv", Address = new Uri("http://localhost:8085") } };
        services.AddSingleton(options);

        ServiceProvider provider = services.BuildServiceProvider();

        var factory = new SamsungTvClientFactory(
            provider.GetService<IHttpClientFactory>(),
            provider.GetService<IMemoryCache>(),
            options);

        return factory;
    }
}
