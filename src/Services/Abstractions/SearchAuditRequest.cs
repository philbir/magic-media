using System;
using System.Collections.Generic;

namespace MagicMedia
{
    public class SearchAuditRequest
    {
        public int PageNr { get; set; }

        public int PageSize { get; set; }

        public Guid? UserId { get; set; }

        public IEnumerable<string>? Actions { get; set; }

        public bool? Success { get; set; }
    }
}
