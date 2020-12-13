using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Store;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia
{
    public class CloudAIMediaProcessingService : ICloudAIMediaProcessingService
    {
        private readonly IEnumerable<ICloudAIMediaAnalyser> _analysers;
        private readonly IMediaService _mediaService;
        private readonly IMediaStore _mediaStore;

        public CloudAIMediaProcessingService(
            IEnumerable<ICloudAIMediaAnalyser> analysers,
            IMediaService mediaService,
            IMediaStore mediaStore)
        {
            _analysers = analysers;
            _mediaService = mediaService;
            _mediaStore = mediaStore;
        }

        public async Task<MediaAI?> AnalyseMediaAsync(Media media, CancellationToken cancellationToken)
        {
            MediaAI? result = null;

            foreach (ICloudAIMediaAnalyser imageAnalyzer in _analysers)
            {
                MediaAI aiData = await AnalyseMediaAsync(media, imageAnalyzer, cancellationToken);

                result = await SaveMediaAI(aiData, cancellationToken);
            }

            return result;
        }

        private async Task<MediaAI> SaveMediaAI(MediaAI aiData, CancellationToken cancellationToken)
        {
            MediaAI? existing = await _mediaStore.MediaAI.GetByMediaIdAsync(
                aiData.MediaId,
                cancellationToken);

            if (existing == null)
            {
                await _mediaStore.MediaAI.SaveAsync(aiData, cancellationToken);

                return aiData;
            }
            else
            {
                List<MediaAITag> tags = new();
                tags.AddRangeIfNotNull(existing.Tags);
                tags.AddRangeIfNotNull(aiData.Tags);

                List<MediaAIObject> objects = new();
                objects.AddRangeIfNotNull(existing.Objects);
                objects.AddRangeIfNotNull(aiData.Objects);

                List<MediaAISourceInfo> sources = new();
                sources.AddRangeIfNotNull(existing.SourceInfo);
                sources.AddRangeIfNotNull(aiData.SourceInfo);

                existing.Tags = tags;
                existing.Objects = objects;
                existing.SourceInfo = sources;

                if (aiData.Colors != null)
                {
                    existing.Colors = aiData.Colors;
                }

                if (aiData.Caption != null)
                {
                    existing.Caption = aiData.Caption;
                }

                await _mediaStore.MediaAI.SaveAsync(existing, cancellationToken);

                return existing;
            }
        }

        public async Task ProcessNewBySourceAsync(
            AISource source,
            CancellationToken cancellationToken)
        {
            IEnumerable<MediaAI> result = await _mediaStore.MediaAI.GetWithoutSourceInfoAsync(
                source,
                limit: 1,
                excludePersons: true,
                cancellationToken);

            foreach (MediaAI mediaAI in result)
            {
                Media media = await _mediaService.GetByIdAsync(mediaAI.MediaId, cancellationToken);

                ICloudAIMediaAnalyser analyser = _analysers.Single(x => x.Source == source);

                MediaAI aiData = await AnalyseMediaAsync(media, analyser, cancellationToken);

                await SaveMediaAI(aiData, cancellationToken);
            }
        }

        public async Task<MediaAI?> AnalyseMediaAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            Media media = await _mediaService.GetByIdAsync(mediaId, cancellationToken);

            MediaAI? mediaAI = await AnalyseMediaAsync(media, cancellationToken);

            return mediaAI;
        }

        private async Task<MediaAI> AnalyseMediaAsync(
            Media media,
            ICloudAIMediaAnalyser analyser,
            CancellationToken cancellationToken)
        {
            Log.Information("Analyse media {Id} using {Analyzer}", media.Id, analyser.Source);

            MediaBlobData? request = _mediaService.GetBlobRequest(media, MediaFileType.Original);

            using Stream imageStream = _mediaStore.Blob.GetStreamAsync(request);
            using Stream stream = await RemoveExifDataAsync(imageStream, cancellationToken);

            MediaAI aiData = new();

            try
            {
                aiData = await analyser.AnalyseImageAsync(
                        stream,
                        cancellationToken);
            }
            catch (Exception ex)
            {
                aiData.SourceInfo = new List<MediaAISourceInfo>
                {
                    new MediaAISourceInfo
                    {
                        AnalysisDate = DateTime.UtcNow,
                        Source = analyser.Source,
                        Success = false,
                        Data = new()
                        {
                            ["Error"] = ex.Message
                        }
                    }
                };
            }

            aiData.MediaId = media.Id;
            aiData.Id = Guid.NewGuid();

            return aiData;
        }

        private async Task<Stream> RemoveExifDataAsync(
            Stream imageStream,
            CancellationToken cancellationToken)
        {
            Image image = await Image.LoadAsync(imageStream);


            image.Metadata.ExifProfile = null;
            var ms = new MemoryStream();
            Size size = image.Size();

            if (imageStream.Length > 4 * 1024 * 1024)
            {
                var encoder = new JpegEncoder()
                {
                    Quality = 50
                };
                await image.SaveAsJpegAsync(ms, encoder, cancellationToken);
            }
            else
            {
                await image.SaveAsJpegAsync(ms, cancellationToken);
            }

            ms.Position = 0;
            return ms;
        }
    }
}
