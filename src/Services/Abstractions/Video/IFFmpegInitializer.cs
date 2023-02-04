using System.Threading.Tasks;

namespace MagicMedia.Video;

public interface IFFmpegInitializer
{
    Task Intitialize();
}

public class FFmpegOption
{
    public string? Location { get; set; }

    public bool AutoDownload { get; set; }
}
