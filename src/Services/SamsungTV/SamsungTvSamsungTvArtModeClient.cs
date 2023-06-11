using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace MagicMedia.SamsungTv;

public class SamsungTvSamsungTvArtModeClient : ISamsungTvArtModeClient
{
    private readonly string _deviceName;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;

    internal SamsungTvSamsungTvArtModeClient(
        string deviceName,
        IHttpClientFactory httpClientFactory,
        IMemoryCache memoryCache)
    {
        _deviceName = deviceName;
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
    }

    public async Task<bool> IsOnlineAsync(CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory.CreateClient(_deviceName);

        var timoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timoutToken.Token);

        try
        {
            await client.GetAsync("api/online", linkedSource.Token);

            return true;
        }
        catch (TaskCanceledException timeout)
        {
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<IReadOnlyList<SamsungTvMedia>> GetAllMediaAsync(CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory.CreateClient(_deviceName);

        var options = new JsonSerializerOptions();
        options.Converters.Add(new SamsungTvDateTimeConverter());

        IEnumerable<SamsungTvMedia>? response = await client.GetFromJsonAsync<IEnumerable<SamsungTvMedia>>(
            "api/media",
            options: options,
            cancellationToken);

        return response.ToList();
    }

    public async Task<SamsungTvFeatures> GetFeaturesAsync(CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory.CreateClient(_deviceName);

        SamsungTvFeatures? response = await client.GetFromJsonAsync<SamsungTvFeatures>(
            "api/features",
            options: null,
            cancellationToken);

        return response;
    }

    public async Task<byte[]?> GetPreviewAsync(string id, CancellationToken cancellationToken)
    {
        var cacheKey = $"samsung_{_deviceName}_{id}";

        return  await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            HttpClient client = _httpClientFactory.CreateClient(_deviceName);

            return await client.GetByteArrayAsync($"api/preview/{id}.jpg", cancellationToken);
        });
    }

    public async Task<string> UploadAsync(
        byte[] data,
        string filename,
        string? matte,
        CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory.CreateClient(_deviceName);

        using var form = new MultipartFormDataContent();
        using var imageContent = new ByteArrayContent(data);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

        form.Add(imageContent, "image", filename);

        form.Add(new StringContent("matte"), matte ?? "none");

        HttpResponseMessage response = await client.PostAsync("api/upload", form, cancellationToken);
        response.EnsureSuccessStatusCode();

        UploadImageResult? result = await response.Content.ReadFromJsonAsync<UploadImageResult>(
            cancellationToken: cancellationToken);

        return result.Id;
    }

    public async Task SelectImageAsync(string id, CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory.CreateClient(_deviceName);

        HttpResponseMessage response = await client.PostAsync(
            $"api/select/{id}",
            content: null,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task ChangeMatteAsync(string id, string matte, CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory.CreateClient(_deviceName);

        HttpResponseMessage response = await client.PostAsync(
            $"api/matte/{id}/{matte}",
            content: null,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }


    public async Task SetFilterAsync(string id, string filter, CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory.CreateClient(_deviceName);

        HttpResponseMessage response = await client.PostAsync(
            $"api/filter/{id}/{filter}",
            content: null,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory.CreateClient(_deviceName);

        HttpResponseMessage response = await client.PostAsync(
            $"api/delete/{id}",
            content: null,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}

public class UploadImageResult
{
    [JsonPropertyName("content_id")] public string Id { get; set; }
}
