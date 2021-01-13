using System;
using System.Collections.Generic;
using MagicMedia.Store;

namespace MagicMedia
{
    public class SimilarMediaGroup
    {
        public Guid Id { get; set; }

        public MediaHashType HashType { get; set; }

        public int Count { get; set; }

        public IEnumerable<Guid> MediaIds { get; set; }
    }
}
