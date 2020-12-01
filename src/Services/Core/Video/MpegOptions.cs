using System.IO;
using System.Threading.Tasks;
using Serilog;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace MagicMedia.Video
{
    public class FFmpegInitializer : IFFmpegInitializer
    {
        private readonly FFmpegOption _options;

        public FFmpegInitializer(FFmpegOption options)
        {
            _options = options;
        }

        public async Task Intitialize()
        {
            Log.Information("Initialize FFmpeg");
            if (_options.AutoDownload)
            {
                Log.Information("FFmpeg GetLatestVersion");
                await FFmpegDownloader.GetLatestVersion(
                    FFmpegVersion.Official,
                    GetDirectory());
            }
            else
            {
                FFmpeg.SetExecutablesPath(GetDirectory());
            }
        }

        private string GetDirectory()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), _options.Location ?? "ffmpeg");
        }
    }
}
