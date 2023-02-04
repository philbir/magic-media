using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SampleDataSeeder
{
    public class UnsplashRandomDataSource : ISampleDataSource
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UnsplashRandomDataSource(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<SampleMedia> LoadAsync(CancellationToken cancellationToken)
        {
            HttpClient client = _httpClientFactory.CreateClient("Unsplash");

            HttpResponseMessage res = await client.GetAsync("random", cancellationToken);

            byte[] image = await res.Content.ReadAsByteArrayAsync();

            return new SampleMedia
            {
                Data = image,
                Filename = $"Unsplash_{DateTime.Now.Ticks}.jpg"
            };
        }
    }
}
