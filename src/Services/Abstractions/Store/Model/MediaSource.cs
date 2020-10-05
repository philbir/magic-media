using System;

namespace MagicMedia.Store
{
    public class MediaSource
    {
        public string Type { get; set; }

        public string Identifier { get; set; }

        public DateTime ImportedAt { get; set; }
    }
}
