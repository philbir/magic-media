using System;

namespace MagicMedia.GraphQL.Face
{
    public partial class FaceMutations
    {
        public class PredictPersonInput
        {
            public Guid FaceId { get; set; }

            public double? Distance { get; set; }
        }
    }
}
