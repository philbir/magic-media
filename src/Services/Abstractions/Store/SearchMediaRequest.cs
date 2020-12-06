using System;
using System.Collections.Generic;

namespace MagicMedia.Store
{
    public class SearchMediaRequest
    {
        public int PageSize { get; set; }

        public int PageNr { get; set; }

        public IEnumerable<Guid>? Persons { get; set; }

        public IEnumerable<string>? Countries { get; set; }

        public IEnumerable<string>? Cities { get; set; }

        public IEnumerable<Guid>? Cameras { get; set; }

        public IEnumerable<MediaType>? MediaTypes { get; set; }

        public string? Folder { get; set; }

        public Guid? AlbumId { get; set; }

        public GeoRadiusFilter? GeoRadius { get; set; }

        public string? Date { get; set; }
    }

    public class GeoRadiusFilter
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Distance { get; set; }
    }
}
