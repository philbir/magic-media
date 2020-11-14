using System;
using System.Collections.Generic;

namespace MagicMedia.Store
{
    public class MediaFace
    {
        public Guid Id { get; set; }

        public Guid MediaId { get; set; }

        public ImageBox Box { get; set; }

        public MediaThumbnail Thumbnail { get; set; }

        public Guid? PersonId { get; set; }

        public FaceRecognitionType RecognitionType { get; set; }

        public FaceState State { get; set; }

        public List<Guid> FalsePositivePersons{ get; set; }

        public double? DistanceThreshold { get; set; }

        public IEnumerable<double> Encoding { get; set; }

        public int? Age { get; set; }
    }

    public record UpdateAgeRequest(Guid Id, int? Age);

}
