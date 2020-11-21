using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store.MongoDb;
using MagicMedia.Video;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace MagicMedia.Playground
{
    public class VideoConverter
    {
        private readonly MediaStoreContext _storeContext;
        private readonly IMediaBlobStore _mediaBlobStore;
        private readonly IVideoProcessingService _videoProcessing;

        public VideoConverter(
            MediaStoreContext storeContext,
            IMediaBlobStore mediaBlobStore,
            IVideoProcessingService videoProcessing)
        {
            _storeContext = storeContext;
            _mediaBlobStore = mediaBlobStore;
            _videoProcessing = videoProcessing;
        }

        public async Task GenerateVideosAsync(CancellationToken cancellationToken)
        {
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);

            List<Store.Media> videos = await _storeContext.Medias.AsQueryable()
                .Where(x => x.MediaType == Store.MediaType.Video)
                .ToListAsync(cancellationToken);

            var todo = videos.Count;

            var videoPreview = @"H:\Drive\Moments\System\VideoPreview";

            foreach (Store.Media video in videos)
            {
                todo--;

                var filename = _mediaBlobStore.GetFilename(new MediaBlobData
                {
                    Directory = video.Folder,
                    Filename = video.Filename,
                    Type = MediaBlobType.Media
                });

                try
                {

                    var outfile = Path.Combine(videoPreview, $"720P_{video.Id}.mp4");

                    if (!File.Exists(outfile))
                    {
                        await _videoProcessing.ConvertTo720Async(filename, outfile, cancellationToken);
                        Console.WriteLine($"{todo} -  Video created: {Path.GetFileName(outfile)}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in Media: {video.Id}. --> {ex.Message}");
                }
            }
        }
    }
}
