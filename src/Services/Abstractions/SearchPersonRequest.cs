using System;
using System.Collections.Generic;

namespace MagicMedia
{
    public class SearchPersonRequest
    {
        public int PageNr { get; set; }

        public int PageSize { get; set; }

        public IEnumerable<Guid>? Groups { get; set; }

        public string SearchText { get; set; }
    }
}
