using System;

namespace MagicMedia.Store
{
    public class VideoInfo
    {
        public string Format { get; set; }
        public double FrameRate { get; set; }
        public double Bitrate { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
