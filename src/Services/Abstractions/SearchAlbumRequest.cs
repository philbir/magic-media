using System;
using System.Collections.Generic;

namespace MagicMedia
{
    public class SearchAlbumRequest
    {
        public int PageNr  { get; set; }

        public int PageSize { get; set; }

        public IEnumerable<Guid>? Persons { get; set; }

        public string SearchText { get; set; }
    }
}
