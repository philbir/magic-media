using System;
using System.Collections.Generic;

namespace MagicMedia.Store
{
    public class SearchMediaRequest
    {
        public int PageSize { get; set; }

        public int PageNr { get; set; }

        public ThumbnailSizeName ThumbnailSize { get; set; }

        public IEnumerable<Guid>? Persons { get; set; }

        public IEnumerable<string>? Countries { get; set; }

        public IEnumerable<string>? Cities { get; set; }
    }
}
