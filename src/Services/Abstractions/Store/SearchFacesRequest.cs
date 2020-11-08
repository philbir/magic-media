using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Store
{
    public class SearchFacesRequest
    {
        public int PageSize { get; set; } = 100;

        public int PageNr { get; set; }

        public IEnumerable<FaceState>? States { get; set; }

        public IEnumerable<FaceRecognitionType>? RecognitionTypes { get; set; }

        public IEnumerable<Guid>? Persons { get; set; }
    }
}
