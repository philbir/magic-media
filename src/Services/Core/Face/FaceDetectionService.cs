using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Face
{
    public class FaceDetectionService : IFaceDetectionService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FaceDetectionService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<FaceDetectionResponse>> DetectFacesAsync(
            Stream stream,
            CancellationToken cancellationToken)
        {
            HttpClient client = _httpClientFactory.CreateClient("Face");
            var request = new HttpRequestMessage(HttpMethod.Post, "face/detect");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StreamContent(stream), "file", "detectme.jpg");
            request.Content = multipart;

            HttpResponseMessage res = await client.SendAsync(request, cancellationToken);

            if (res.IsSuccessStatusCode)
            {
                IEnumerable<FaceDetectionResponse>? faces = await res.Content
                    .ReadFromJsonAsync<IEnumerable<FaceDetectionResponse>>(JsonSettings);

                return faces ?? new List<FaceDetectionResponse>();
            }
            else
            {
                string text = await res.Content.ReadAsStringAsync();
                throw new ApplicationException(text);
            }
        }

        public async Task<BuildFaceModelResult?> BuildModelAsync(
            IEnumerable<PersonEncodingData> encodings,
            CancellationToken cancellationToken)
        {
            HttpClient client = _httpClientFactory.CreateClient("Face");
            var request = new HttpRequestMessage(HttpMethod.Post, "face/buildmodel");

            request.Content = new StringContent(
                JsonSerializer.Serialize(encodings, JsonSettings),
                System.Text.Encoding.UTF8,
                "application/json");

            HttpResponseMessage res = await client.SendAsync(request, cancellationToken);

            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<BuildFaceModelResult>();
            }
            else
            {
                string text = await res.Content.ReadAsStringAsync();
                throw new ApplicationException(text);
            }
        }

        public async Task<Guid?> PredictPersonAsync(
            PredictPersonRequest request,
            CancellationToken cancellationToken)
        {
            HttpClient client = _httpClientFactory.CreateClient("Face");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "face/predict");

            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(request, JsonSettings),
                System.Text.Encoding.UTF8,
                "application/json");

            HttpResponseMessage res = await client.SendAsync(httpRequest, cancellationToken);

            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<Guid?>();
            }
            else
            {
                string text = await res.Content.ReadAsStringAsync();
                throw new ApplicationException(text);
            }
        }

        private JsonSerializerOptions JsonSettings => new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
