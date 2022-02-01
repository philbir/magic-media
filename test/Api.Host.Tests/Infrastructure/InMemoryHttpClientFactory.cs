using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MagicMedia.Api.Host.Tests.Infrastructure
{
    public class InMemoryHttpClientFactory
    {
        public HttpClient HttpClient { get; set; }

        public Func<Task<string>> TokenResolver { get; set; }

        public HttpClient CreateClient(string name)
        {
            if (TokenResolver != null)
            {
                var token = TokenResolver().GetAwaiter().GetResult();
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "bearer", token);
            }
            return HttpClient;
        }
    }
}
