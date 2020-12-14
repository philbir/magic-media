using System;
using System.Collections.Generic;

namespace MagicMedia.ImageAI
{
    public class ImageAIDetectionResult
    {
        public Guid MediaId { get; set; }

        public bool Success { get; set; }

        public string Error { get; set; }

        public IEnumerable<ImageAIDetectionItem>? Items { get; set; }
    }

    public class ImageAIDetectionItem
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public double Probability { get; set; }

        public ImageBox? Box { get; set; }
    }
}
