using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using NGeoHash;
using Serilog;
using Xabe.FFmpeg;
using MetadataEx = MetadataExtractor;

namespace MagicMedia.Video
{
    public class VideoProcessingService : IVideoProcessingService
    {
        private const string QuickTimeDateFormat = "ddd MMM dd HH:mm:ss yyyy";
        private readonly IGeoDecoderService _geoDecoderService;

        public VideoProcessingService(
            IGeoDecoderService geoDecoderService)
        {
            _geoDecoderService = geoDecoderService;
        }

        public async Task<ExtractVideoDataResult> ExtractVideoDataAsync(
            string filename,
            CancellationToken cancellationToken)
        {
            Log.Information("Extract VideoData for: {Filename}", filename);

            ExtractVideoDataResult result = new ExtractVideoDataResult()
            {
                Meta = new MediaMetadata()
            };


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filename, cancellationToken);
            IVideoStream? videoStream = mediaInfo.VideoStreams.FirstOrDefault();
            if (videoStream != null)
            {
                result.Info = new VideoInfo
                {
                    Bitrate = videoStream.Bitrate,
                    Format = videoStream.PixelFormat,
                    FrameRate = videoStream.Framerate,
                    Duration = videoStream.Duration
                };

                result.Size = mediaInfo.Size;

                result.Meta.Dimension = new MediaDimension
                {
                    Width = videoStream.Width,
                    Height = videoStream.Height
                };

                videoStream.SetCodec(VideoCodec.png);

                var tmpFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");

                try
                {
                    IConversionResult cr = await FFmpeg.Conversions.New()
                        .AddStream(videoStream)
                        .ExtractNthFrame(3, (n) => tmpFile)
                        .Start();

                    result.ImageData = await File.ReadAllBytesAsync(tmpFile, cancellationToken);
                    File.Delete(tmpFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                IReadOnlyList<MetadataEx.Directory>? meta = MetadataEx.ImageMetadataReader
                    .ReadMetadata(filename);

                result.Meta.DateTaken = GetDateTaken(meta);
                result.Meta.GeoLocation = await GetGpsLocation(meta, cancellationToken);
            }

            return result;
        }

        public async Task<string> GeneratePreviewGifAsync(
            string filename,
            string? outfile,
            CancellationToken cancellationToken)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filename, cancellationToken);

            IVideoStream video = mediaInfo.VideoStreams.FirstOrDefault();

            var frames = (video.Duration.TotalSeconds == 0 ? 1 : video.Duration.TotalSeconds) * video.Framerate;

            outfile = outfile ?? Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.gif");

            var totalFrames = 50.0;

            var rate = Math.Ceiling(frames / totalFrames / video.Duration.TotalSeconds);
            if (rate > video.Duration.TotalSeconds)
            {
                rate = video.Duration.TotalSeconds;
            }

            video
                .SetFramerate(rate)
                .SetCodec(VideoCodec.gif)
                .ChangeSpeed(2)
                .SetSize(VideoSize.Hd480);

            IConversion? conversion = FFmpeg.Conversions.New()
                .AddStream(video)
                .SetOutput(outfile);

            await conversion.Start();

            return outfile;
        }

        public async Task<string> ConvertToWebMAsync(
            string filename,
            string? outfile,
            CancellationToken cancellationToken)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filename, cancellationToken);

            IStream video = mediaInfo.VideoStreams.FirstOrDefault()
                .SetCodec(VideoCodec.vp8)
                .SetSize(VideoSize.Hd1080);

            IAudioStream audio = mediaInfo.AudioStreams.FirstOrDefault()
                .SetCodec(AudioCodec.libvorbis);

            outfile = outfile ?? Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.webm");

            IConversion? conversion = FFmpeg.Conversions.New()
                .AddStream(video, audio)
                .SetOutput(outfile);

            await conversion.Start(cancellationToken);

            return outfile;
        }

        public async Task<string> ConvertTo720Async(
            string filename,
            string? outfile,
            CancellationToken cancellationToken)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filename, cancellationToken);

            IStream video = mediaInfo.VideoStreams.FirstOrDefault()
                .SetSize(VideoSize.Hd720);

            IAudioStream audio = mediaInfo.AudioStreams.FirstOrDefault();

            outfile = outfile ?? Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.mp4");

            IConversion? conversion = FFmpeg.Conversions.New()
                .AddStream(video, audio)
                .SetOutput(outfile);

            await conversion.Start(cancellationToken);

            return outfile;
        }

        private async Task<GeoLocation?> GetGpsLocation(IReadOnlyList<MetadataEx.Directory> meta, CancellationToken cancellationToken)
        {
            var geo = GetMetadataValue(meta, "QuickTime Metadata Header/GPS Location");

            if (geo != null)
            {
                var regex = new Regex(@"(\+|\-)(\d{2,}\.\d{2,})");
                MatchCollection? matches = regex.Matches(geo);

                var coordintates = new List<double>();

                foreach (string? value in matches.Select(x => x?.ToString()))
                {
                    double coordValue;
                    if (double.TryParse(value, out coordValue))
                    {
                        coordintates.Add(coordValue);
                    }
                }

                if (coordintates.Count == 2)
                {
                    var gps = new GeoLocation()
                    {
                        Type = "gps",
                        Point = GeoPoint.Create(coordintates[0], coordintates[1])
                    };
                    try
                    {
                        gps.GeoHash = GeoHash.Encode(gps.Point.Coordinates[1], gps.Point.Coordinates[0]);
                        gps.Address = await _geoDecoderService.DecodeAsync(
                            coordintates[0],
                            coordintates[1],
                            cancellationToken);

                        return gps;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return null;
        }

        private DateTime? GetDateTaken(IReadOnlyList<MetadataEx.Directory> meta)
        {
            string? dateTime = GetMetadataValue(meta, "QuickTime Movie Header/Created");

            if (dateTime != null)
            {
                DateTime parsed;
                if (DateTime.TryParseExact(
                    dateTime,
                    QuickTimeDateFormat,
                    null,
                    DateTimeStyles.None,
                    out parsed))
                {
                    return parsed;
                }
            }

            return null;
        }

        private string? GetMetadataValue(
            IReadOnlyList<MetadataEx.Directory>? meta, string path)
        {
            if (meta == null)
            {
                return null;
            }

            var frags = path.Split('/');

            MetadataEx.Directory? directory = meta
                .FirstOrDefault(x => x.Name == frags[0]);

            if (directory != null)
            {
                MetadataEx.Tag? tag = directory.Tags.FirstOrDefault(x => x.Name == frags[1]);
                if (tag != null)
                {
                    return tag.Description;
                }
            }

            return null;
        }
    }
}
