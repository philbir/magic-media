using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace MagicMedia.Video;

public class FFmpegInitializer(
    FFmpegOption options,
    ILogger<FFmpegInitializer> logger) : IFFmpegInitializer
{
    public async Task Intitialize()
    {
        var location = GetDirectory();

        //logger.InitializeFFmpeg(location);
        //Log.Information("Initialize FFmpeg with location: {Location}", location);
        if (options.AutoDownload)
        {
            //logger.
            //Log.Information("FFmpeg GetLatestVersion");
            await FFmpegDownloader.GetLatestVersion(
                FFmpegVersion.Official,
                location);
        }

        FFmpeg.SetExecutablesPath(location);
    }

    private string GetDirectory()
    {
        if (options.Location == null)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
        }
        else
        {
            return options.Location;
        }
    }
}

public static partial class FFmpegInitializerLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Initialize FFmpeg with location: {Location}")]
    public static partial void InitializeFFmpeg(ILogger logger, string location);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "FFmpeg GetLatestVersion")]
    public static partial void FFmpegGetLatestVersion(ILogger logger);
}
