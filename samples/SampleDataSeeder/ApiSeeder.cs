using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.SampleDataSeader
{
    public class ApiSeeder
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiSeeder(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SeedFromDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            var dir = new DirectoryInfo(path);

            foreach (FileInfo file in dir.GetFiles("*.jpg"))
            {
                Console.WriteLine($"Uploading {file.Name}");
                try
                {
                    await UploadAsync(file, cancellationToken);

                }
                catch  (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        public async Task UploadAsync(FileInfo file, CancellationToken cancellationToken)
        {
            HttpClient httpClient = _httpClientFactory
                .CreateClient("MagicMedia");

            using FileStream stream = file.OpenRead();

            var request = new HttpRequestMessage(HttpMethod.Post, "media/upload");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StreamContent(stream), "file", file.Name);
            request.Content = multipart;

            HttpResponseMessage res = await httpClient.SendAsync(request, cancellationToken);
        }

    }
}
