using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;

namespace MagicMedia.Telemetry;

public class TelemetryOptions
{
    public string ServiceName { get; set; }

    public ElasticServerOptions ElasticServer { get; set; }
}


public class ElasticServerOptions
{
    public string Url { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
}
