using System.Collections.Generic;

namespace MagicMedia.Face
{
    public class PredictPersonRequest
    {
        IEnumerable<double> Encoding { get; set; }

        public double Distance { get; set; }
    }
}
