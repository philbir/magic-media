using System.Collections.Generic;

namespace MagicMedia.Store
{
    public class MediaAISummary
    {
        public IEnumerable<AISource>? Sources { get; set; }

        public int ObjectCount { get; set; }

        public int PersonCount { get; set; }

        public int TagCount { get; set; }
    }
}
