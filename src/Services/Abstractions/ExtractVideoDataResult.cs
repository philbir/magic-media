using MagicMedia.Store;

namespace MagicMedia;

public class ExtractVideoDataResult
{
    public VideoInfo? Info { get; set; }

    public MediaDimension? Dimensions { get; set; }

    public byte[]? ImageData { get; set; }

    public MediaMetadata? Meta { get; set; }
    public long Size { get; set; }
}
