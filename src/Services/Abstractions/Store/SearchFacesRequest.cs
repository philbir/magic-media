using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Store
{
    public class SearchFacesRequest
    {
        public IEnumerable<FaceState>? States { get; set; }
    }

}
