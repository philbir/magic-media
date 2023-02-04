using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Face;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using MagicMedia.Video;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;
using SixLabors.ImageSharp;
using MetadataEx = MetadataExtractor;

namespace MagicMedia.Playground
{
    public class BulkMediaUpdater
    {
        private readonly IMediaService _mediaService;
        private readonly IFaceService _faceService;
        private readonly IMetadataExtractor _metadataExtractor;
        private readonly IGeoDecoderService _geoDecoderService;
        private readonly IMediaBlobStore _blobStore;
        private readonly MediaStoreContext _dbContext;

        public BulkMediaUpdater(
            IMediaService mediaService,
            IFaceService faceService,
            IMetadataExtractor metadataExtractor,
            IGeoDecoderService geoDecoderService,
            IMediaBlobStore blobStore,
            MediaStoreContext dbContext)
        {
            _mediaService = mediaService;
            _faceService = faceService;
            _metadataExtractor = metadataExtractor;
            _geoDecoderService = geoDecoderService;
            _blobStore = blobStore;
            _dbContext = dbContext;
        }

        public async Task UpdateMediaAISummaryAsync(CancellationToken cancellationToken)
        {
            List<MediaAI> allAI = await _dbContext.MediaAI.AsQueryable().ToListAsync(cancellationToken);

            foreach (List<MediaAI> chunk in allAI.ChunkBy(500))
            {
                List<UpdateOneModel<Media>> bulkUpdates = new();

                foreach (MediaAI mediaAI in chunk)
                {
                    MediaAISummary summary = BuildSummary(mediaAI);

                    UpdateDefinition<Media> update = Builders<Media>.Update.Set(x => x.AISummary, summary);

                    bulkUpdates.Add(new UpdateOneModel<Media>(
                        Builders<Media>.Filter.Eq(x => x.Id, mediaAI.MediaId),
                        update));

                }

                await _dbContext.Medias.BulkWriteAsync(bulkUpdates, null, cancellationToken);
                Console.WriteLine("Chunks updated...");
            }
        }

        private MediaAISummary BuildSummary(MediaAI mediaAI)
        {
            return new MediaAISummary
            {
                Sources = mediaAI.SourceInfo.Select(x => x.Source),
                ObjectCount = mediaAI.Objects.Count(x => !x.Name.Equals("person", StringComparison.InvariantCultureIgnoreCase)),
                PersonCount = mediaAI.Objects.Count(x => x.Name.Equals("person", StringComparison.InvariantCultureIgnoreCase)),
                TagCount = mediaAI.Tags.Count()
            };
        }

        public async Task UpdateLocationAsync(CancellationToken cancellationToken)
        {
            List<Guid> ids = await _dbContext.Medias.AsQueryable()
                .Where(x => x.State == MediaState.Active &&
                            x.MediaType == MediaType.Video &&
                            x.GeoLocation.Point != null &&
                            x.GeoLocation.Address == null)
                .OrderByDescending(x => x.Source.ImportedAt)
                .Take(10000)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            VideoProcessingService videoProcessingService = new VideoProcessingService(_geoDecoderService);

            foreach (Guid id in ids)
            {
                Console.WriteLine($"{id}");
                try
                {
                    Media media = await _mediaService.GetByIdAsync(id, cancellationToken);
                    MediaBlobData file = await _mediaService.GetMediaData(media, cancellationToken);
                    

                    MediaMetadata meta = null;
                    if (media.MediaType == MediaType.Image)
                    {
                        Image img = Image.Load(file.Data);
                        meta = await _metadataExtractor.GetMetadataAsync(img, cancellationToken);
                    }
                    else
                    {
                        IReadOnlyList<MetadataEx.Directory>? vmeta = MetadataEx.ImageMetadataReader
                            .ReadMetadata(_blobStore.GetFilename(file));

                        meta = new MediaMetadata();

                        meta.DateTaken = videoProcessingService.GetDateTaken(vmeta);
                        meta.GeoLocation = await videoProcessingService.GetGpsLocation(vmeta, cancellationToken);
                    }

                    await _dbContext.Medias.UpdateOneAsync(x => x.Id == id,
                        Builders<Media>.Update.Set(x => x.GeoLocation, meta.GeoLocation),
                        new UpdateOptions(),
                        cancellationToken);

                    Console.WriteLine($"{meta.GeoLocation?.Address?.Name}");
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task DeleteMediaAIOrphansAsync()
        {
            List<Guid> allIds = _dbContext.Medias.AsQueryable()
                            .Where(x => x.State == MediaState.Active)
                            .Select(x => x.Id)
                            .ToList();

            FilterDefinition<MediaAI> filter = Builders<MediaAI>.Filter.Not(
                Builders<MediaAI>.Filter.In(x => x.MediaId, allIds));

            IFindFluent<MediaAI, MediaAI> cursor = _dbContext.MediaAI.Find(filter);

            Guid[] mediaAIds = (await cursor.ToListAsync()).Select(x => x.Id).ToArray();

            _dbContext.MediaAI.DeleteMany(f =>
                mediaAIds.Contains(f.Id));
        }

        public async Task ResetMediaAIErrorsAsync()
        {
            AISource soruce = AISource.ImageAI;

            FilterDefinition<MediaAISourceInfo> elmFilter = Builders<MediaAISourceInfo>.Filter.And(
                Builders<MediaAISourceInfo>.Filter.Eq(x => x.Source, soruce),
                Builders<MediaAISourceInfo>.Filter.Eq(x => x.Success, false));

            FilterDefinition<MediaAI> filter = Builders<MediaAI>.Filter.ElemMatch(x => x.SourceInfo, elmFilter);

            IFindFluent<MediaAI, MediaAI> cursor = _dbContext.MediaAI.Find(filter);

            List<MediaAI> mediaIdList = await cursor.Limit(10000).ToListAsync();

            int todo = mediaIdList.Count;

            foreach (MediaAI mediaAI in mediaIdList)
            {
                Console.WriteLine($"{todo} - {mediaAI.MediaId}");

                if (mediaAI.SourceInfo.Count() == 1)
                {
                    await _dbContext.MediaAI.DeleteOneAsync(x => x.Id == mediaAI.Id);
                }
                else
                {
                    mediaAI.SourceInfo = mediaAI.SourceInfo.Where(x => x.Source != soruce);
                    mediaAI.Tags = mediaAI.Tags.Where(x => x.Source != soruce);
                    mediaAI.Objects = mediaAI.Objects.Where(x => x.Source != soruce);

                    await _dbContext.MediaAI.ReplaceOneAsync(
                        x => x.Id == mediaAI.Id,
                        mediaAI, new ReplaceOptions());
                }

                todo--;
            }
        }

        public async Task CleanUpDeletedAsync(CancellationToken cancellationToken)
        {
            FilterDefinition<Media> filter = Builders<Media>.Filter.Regex(
                                    x => x.Folder,
                                    new BsonRegularExpression("^Deleted", "i"));

            IAsyncCursor<Media> cursor = await _dbContext.Medias
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
