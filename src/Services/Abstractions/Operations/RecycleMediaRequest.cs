using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace MagicMedia.Operations
{
    public record RecycleMediaRequest(IEnumerable<Guid> Ids, string OperationId);

    public class UpdateMediaMetadataRequest
    {
        public IEnumerable<Guid> Ids { get; init; }

        public string OperationId { get; init; }

        public DateTimeOffset? DateTaken { get; set; }

        public UpdateMedataGeoLocation GeoLocation { get; set; }
    }

    public class UpdateMedataGeoLocation
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? CountryCode { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public string? Distric1 { get; set; }
        public string? Distric2 { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
