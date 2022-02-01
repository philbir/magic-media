using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MagicMedia.Identity.Core.Services;

public class ECallSmsService : ISmsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ECallOptions _options;

    public ECallSmsService(
        IHttpClientFactory httpClientFactory,
        ECallOptions options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    public async Task SendSmsAsync(
        string mobile,
        string message,
        CancellationToken cancellationToken)
    {
        var jobId = Guid.NewGuid();
        HttpClient httpClient = _httpClientFactory.CreateClient("ECall");

        HttpResponseMessage result = await httpClient.PostAsync(
            "sms",
            CreateSendSmsRequest(mobile, message, jobId),
            cancellationToken);

        result.EnsureSuccessStatusCode();

        using Stream stream = await result.Content.ReadAsStreamAsync();
        ECallResult resultContent = DeserializeResult(stream);
    }

    private HttpContent CreateSendSmsRequest(string mobileNr, string message, Guid jobId)
    {
        return new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", _options.Username),
                new KeyValuePair<string, string>("password", _options.Password),
                new KeyValuePair<string, string>("callBack", _options.From),
                new KeyValuePair<string, string>("address", mobileNr),
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("jobId", jobId.ToString("N")),
            });
    }

    private ECallResult DeserializeResult(Stream resultStream)
    {
        var serializer = new XmlSerializer(typeof(ECallResult));
        return (ECallResult)serializer.Deserialize(resultStream);
    }


    [XmlRoot(
        "Result",
        Namespace = "http://schemas.datacontract.org/2004/07/eCallHttp.Models",
        IsNullable = true)]
    public class ECallResult
    {
        [XmlElement("ResultCode")]
        public string? ResultCode { get; set; }

        [XmlElement("ResultText")]
        public string? ResultText { get; set; }
    }
}
