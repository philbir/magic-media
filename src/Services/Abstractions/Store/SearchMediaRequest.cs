using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace MagicMedia.Store
{
    public class SearchMediaRequest
    {
        public int PageSize { get; set; }

        public int PageNr { get; set; }

        public IEnumerable<Guid>? Persons { get; set; }

        public IEnumerable<string>? Countries { get; set; }

        public IEnumerable<string>? Cities { get; set; }

        public string? Folder { get; set; }

        public Guid? AlbumId { get; set; }
    }
}
