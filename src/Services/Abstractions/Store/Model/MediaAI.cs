using System;
using System.Collections.Generic;

namespace MagicMedia.Store;

public class MediaAI
{
    public Guid Id { get; set; }

    public Guid MediaId { get; set; }

    public MediaAICaption? Caption { get; set; }

    public IEnumerable<MediaAITag> Tags { get; set; } = new List<MediaAITag>();

    public IEnumerable<MediaAIObject> Objects { get; set; } = new List<MediaAIObject>();

    public MediaAIColors? Colors { get; set; }

    public IEnumerable<MediaAISourceInfo> SourceInfo { get; set; } = new List<MediaAISourceInfo>();

    public int PersonCount { get; set; }
}
