using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MagicMedia.Thumbprint
{
    public class GeoIpLocationResponse
    {
        public string? Ip { get; set; }

        [JsonPropertyName("continent_code")]
        public string? ContinentCode { get; set; }

        [JsonPropertyName("continent_name")]
        public string? ContinentName { get; set; }

        [JsonPropertyName("country_code2")]
        public string? CountryCode2 { get; set; }

        [JsonPropertyName("country_code3")]
        public string? CountryCode3 { get; set; }

        [JsonPropertyName("country_name")]
        public string? CountryName { get; set; }

        [JsonPropertyName("country_capital")]
        public string? CountryCapital { get; set; }

        [JsonPropertyName("state_prov")]
        public string? StateProv { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string? Zipcode { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [JsonPropertyName("is_eu")]
        public bool IsEU { get; set; }

        [JsonPropertyName("calling_code")]
        public string? CallingCode { get; set; }

        [JsonPropertyName("country_tld")]
        public string? CountryTld { get; set; }

        public string? Languages { get; set; }

        [JsonPropertyName("country_flag")]
        public string? CountryFlag { get; set; }

        [JsonPropertyName("geoname_id")]
        public string? GeonameId { get; set; }

        public string? Isp { get; set; }

        [JsonPropertyName("connection_type")]
        public string? Connection_Type { get; set; }

        public string? Organization { get; set; }

        public Currency? Currency { get; set; }

        [JsonPropertyName("time_zone")]
        public TimeZone? TimeZone { get; set; }
    }

    public class Currency
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Symbol { get; set; }
    }

    public class TimeZone
    {
        public string? Name { get; set; }
        public int Offset { get; set; }

        [JsonPropertyName("current_time")]
        public DateTime? CurrentTime { get; set; }

        [JsonPropertyName("current_time_unix")]
        public float CurrentTimeUnix { get; set; }

        [JsonPropertyName("is_dst")]
        public bool IsDst { get; set; }

        [JsonPropertyName("dst_savings")]
        public int DstSavings { get; set; }
    }
}

/*
{
  "ip": "85.4.148.81",
  "continent_code": "EU",
  "continent_name": "Europe",
  "country_code2": "CH",
  "country_code3": "CHE",
  "country_name": "Switzerland",
  "country_capital": "Bern",
  "state_prov": "Zürich",
  "district": "Zürich",
  "city": "Zürich",
  "zipcode": "8045",
  "latitude": "47.36194",
  "longitude": "8.50966",
  "is_eu": false,
  "calling_code": "+41",
  "country_tld": ".ch",
  "languages": "de-CH,fr-CH,it-CH,rm",
  "country_flag": "https://ipgeolocation.io/static/flags/ch_64.png",
  "geoname_id": "10392091",
  "isp": "Swisscom (Schweiz) AG",
  "connection_type": "",
  "organization": "Swisscom (Schweiz) AG",
  "currency": {
    "code": "CHF",
    "name": "Swiss Franc",
    "symbol": "CHF"
  },
  "time_zone": {
    "name": "Europe/Zurich",
    "offset": 1,
    "current_time": "2020-12-29 08:18:03.242+0100",
    "current_time_unix": 1609226283.242,
    "is_dst": false,
    "dst_savings": 1
  }
}

*/
