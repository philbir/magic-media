using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Face
{
    public class PersonEncodingData
    {
        public IEnumerable<double> Encoding { get; init; }

        public Guid PersonId { get; init; }
    }
}
