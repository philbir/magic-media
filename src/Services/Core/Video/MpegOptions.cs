using System.IO;
using System.Threading.Tasks;
using Serilog;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace MagicMedia.Video;

public class FFmpegInitializer : IFFmpegInitializer
{
    private readonly FFmpegOption _options;

    public FFmpegInitializer(FFmpegOption options)
    {
        _options = options;
    }

    public async Task Intitialize()
    {
        var location = GetDirectory();

        //Log.Information("Initialize FFmpeg with location: {Location}", location);
        if (_options.AutoDownload)
        {
            //Log.Information("FFmpeg GetLatestVersion");
            await FFmpegDownloader.GetLatestVersion(
                FFmpegVersion.Official,
                location);
        }

        FFmpeg.SetExecutablesPath(location);
    }

    private string GetDirectory()
    {
        if (_options.Location == null)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
        }
        else
        {
            return _options.Location;
        }
    }
}
