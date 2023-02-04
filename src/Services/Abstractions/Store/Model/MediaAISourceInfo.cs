using System;
using System.Collections.Generic;

namespace MagicMedia.Store;

public class MediaAISourceInfo
{
    public AISource Source { get; set; }

    public DateTime AnalysisDate { get; set; }

    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    public bool Success { get; set; }
}
