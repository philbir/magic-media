using System.Collections.Generic;
using MagicMedia.Discovery;
using MagicMedia.Store;
using SixLabors.ImageSharp;

namespace MagicMedia.Processing
{
    public class MediaProcessorContext
    {
        public byte[]? OriginalData { get; set; }
        public MediaMetadata? Metadata { get; set; }
        public IEnumerable<ThumbnailResult>? Thumbnails { get; set; }
        public IEnumerable<FaceData> FaceData { get; set; }
        public Image? Image { get; set; }
        public byte[]? WebImage { get; set; }
        public MediaDiscoveryIdentifier File { get; set; }
        public MediaProcessingOptions Options { get; set; }
        public Media Media { get; set; }
        public VideoInfo VideoInfo { get; set; }
        public MediaType MediaType { get; set; }
        public long Size { get; set; }
        public List<MediaHash> Hashes { get; set; }
    }

    public class MediaProcessingOptions
    {
        public SaveMediaFileOptions SaveMedia { get; set; }
    }

    public class SaveMediaFileOptions
    {
        public SaveMediaMode SaveMode { get; set; }

        public SaveMediaSourceAction SourceAction { get; set; }
    }

    public enum SaveMediaMode
    {
        KeepInSource,
        CreateNew,
    }

    public enum SaveMediaSourceAction
    {
        Keep,
        Replace,
        Move,
        Delete
    }
}
