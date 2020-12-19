using System;
using System.Collections.Generic;

namespace MagicMedia
{
    public class UpdateAlbumRequest
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public IEnumerable<Guid> SharedWith { get; set; }
    }
}
