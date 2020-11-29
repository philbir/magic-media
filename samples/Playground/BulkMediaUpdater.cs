using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;

namespace MagicMedia.Playground
{
    public class BulkMediaUpdater
    {
        private readonly IMediaService _mediaService;
        private readonly IFaceService _faceService;
        private readonly MediaStoreContext _context;

        public BulkMediaUpdater(IMediaService mediaService, IFaceService faceService, MediaStoreContext context)
        {
            _mediaService = mediaService;
            _faceService = faceService;
            _context = context;
        }


        public async Task CleanUpDeletedAsync(CancellationToken cancellationToken)
        {
            FilterDefinition<Media> filter = Builders<Media>.Filter.Regex(
                                    x => x.Folder,
                                    new BsonRegularExpression("^Deleted", "i"));

            IAsyncCursor<Media> cursor = await _context.Medias
                .FindAsync(filter, null, cancellationToken);

            List<Media> items = await cursor.ToListAsync(cancellationToken);

            int totalCount = items.Count;
            int completedCount = 0;
            int errorCount = 0;

            Log.Information("{Count} items found", totalCount);


            foreach (Media media in items)
            {
                try
                {
                    Log.Information("{Completed} of {Count} - {Id}", completedCount, totalCount, media.Id);

                    IEnumerable<MediaFileInfo> files = _mediaService.GetMediaFiles(media);

                    Backup(media, files);

                    if (files.Any(x => x.Type == MediaFileType.Original))
                    {
                        continue;
                    }

                    await _faceService.DeleteByMediaIdAsync(media.Id, cancellationToken);
                    await _mediaService.DeleteAsync(media, cancellationToken);

                }
                catch (Exception ex)
                {
                    errorCount++;
                    Log.Error(ex, "Error with {Id}", media.Id);
                }
                finally
                {
                    completedCount++;
                }
            }
        }

        private void Backup(Media media, IEnumerable<MediaFileInfo> files)
        {
            var backupRoot = @"C:\MagicMedia\Backup";

            var json = JsonSerializer.Serialize(media);

            File.WriteAllText(Path.Combine(backupRoot, $"{media.Id}.json"), json);

            foreach (MediaFileInfo file in files)
            {
                File.Copy(
                    Path.Combine(file.Location, file.Filename),
                    Path.Combine(backupRoot, "Media", file.Filename));
            }
        }
    }
}
