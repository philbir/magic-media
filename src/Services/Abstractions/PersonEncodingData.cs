using System;
using System.Collections.Generic;

namespace MagicMedia.Face
{
    public class PersonEncodingData
    {
        public IEnumerable<double> Encoding { get; init; }

        public Guid PersonId { get; init; }
    }
}
