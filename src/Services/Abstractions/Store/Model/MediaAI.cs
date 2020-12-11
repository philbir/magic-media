using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Store
{
    public class MediaAI
    {
        public Guid Id { get; set; }

        public Guid MediaId { get; set; }

        public MediaAICaption Caption { get; set; }

        public IEnumerable<MediaAITag> Tags { get; set; }

        public IEnumerable<MediaAIObject> Objects { get; set; }

        public MediaAIColors Colors { get; set; }
    }

    public class MediaAITag
    {
        public ImageAISource Source { get; set; }

        public string Name { get; set; }

        public double Confidence { get; set; }
    }

    public class MediaAIObject
    {
        public ImageAISource Source { get; set; }

        public string Name { get; set; }

        public double Confidence { get; set; }

        public ImageBox Box { get; set; }
    }

    public class MediaAIColors
    {
        public string DominantForeground { get; set; }
        public string DominantBackground { get; set; }

        public string Accent { get; set; }

        public bool IsBackWhite { get; set; }
    }

    public class MediaAICaption
    {
        public string Text { get; set; }

        public double Confidence { get; set; }
    }

    public enum ImageAISource
    {
        ImageAI,
        AzureCV
    }
}
