using System;
using System.Collections.Generic;

namespace MagicMedia.Operations
{
    public class UpdateMediaMetadataRequest
    {
        public IEnumerable<Guid> Ids { get; set; }

        public string OperationId { get; set; }

        public DateTimeOffset? DateTaken { get; set; }

        public UpdateMedataGeoLocation GeoLocation { get; set; }
    }
}
