using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.ImageAI;
using MagicMedia.Store;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MagicMedia
{
    public class MediaAIService : IMediaAIService
    {
        private readonly IEnumerable<ICloudAIMediaAnalyser> _analysers;
        private readonly IMediaService _mediaService;
        private readonly IMediaStore _mediaStore;

        public MediaAIService(
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
            List<MediaAI> results = new List<MediaAI>();

            foreach (ICloudAIMediaAnalyser imageAnalyzer in _analysers)
            {
                MediaAI aiData = await AnalyseMediaAsync(media, imageAnalyzer, cancellationToken);
                results.Add(aiData);

            }

            MediaAI merged = await SaveMediaAIDatas(results, cancellationToken);

            return merged;
        }


        private MediaAI MergeAIData(MediaAI existing, IEnumerable<MediaAI> aiDatas)
        {
            List<MediaAITag> tags = new();
            tags.AddRangeIfNotNull(existing.Tags);

            List<MediaAIObject> objects = new();
            objects.AddRangeIfNotNull(existing.Objects);

            List<MediaAISourceInfo> sources = new();
            sources.AddRangeIfNotNull(existing.SourceInfo);

            foreach (MediaAI? aiData in aiDatas)
            {
                tags.AddRangeIfNotNull(aiData.Tags);
                objects.AddRangeIfNotNull(aiData.Objects);
                sources.AddRangeIfNotNull(aiData.SourceInfo);

                if (aiData.Colors != null)
                {
                    existing.Colors = aiData.Colors;
                }

                if (aiData.Caption != null)
                {
                    existing.Caption = aiData.Caption;
                }
            }

            existing.Tags = tags;
            existing.Objects = objects;
            existing.SourceInfo = sources;

            return existing;
        }

        public async Task<IEnumerable<Media>> GetMediaIdsForImageAIJobAsync(int limit, CancellationToken cancellationToken)
        {
            IEnumerable<Media> medias = await _mediaStore.GetMediaWithoutAISourceAsync(AISource.ImageAI, limit, cancellationToken);

            return medias;
        }

        private async Task<MediaAI> SaveMediaAIDatas(IEnumerable<MediaAI> aiDatas, CancellationToken cancellationToken)
        {
            MediaAI? existing = await _mediaStore.MediaAI.GetByMediaIdAsync(
                aiDatas.First().MediaId,
                cancellationToken);

            if (existing == null)
            {
                if (aiDatas.Count() == 1)
                {
                    return await SaveMediaAI(aiDatas.Single(), cancellationToken);
                }
                else
                {
                    existing = new MediaAI
                    {
                        Id = Guid.NewGuid(),
                        MediaId = aiDatas.First().MediaId,
                    };
                }
            }

            MediaAI merged = MergeAIData(existing, aiDatas);

            await SaveMediaAI(merged, cancellationToken);

            return merged;
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

                await SaveMediaAIDatas(new[] { aiData }, cancellationToken);
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

        public async Task SaveImageAIDetectionAsync(
            ImageAIDetectionResult imageAIResult,
            CancellationToken cancellationToken)
        {
            var mediaAi = new MediaAI
            {
                Id = Guid.NewGuid(),
                MediaId = imageAIResult.MediaId
            };

            MediaAISourceInfo sourceInfo = new()
            {
                AnalysisDate = DateTime.UtcNow,
                Source = AISource.ImageAI,
                Success = imageAIResult.Success
            };

            if (imageAIResult.Success)
            {
                MapTagsAndObject(imageAIResult, mediaAi);
            }
            else
            {
                sourceInfo.Data.Add("Error", imageAIResult.Error);
            }

            mediaAi.SourceInfo = new List<MediaAISourceInfo> { sourceInfo };

            await SaveMediaAI(mediaAi, cancellationToken);
        }

        private static void MapTagsAndObject(ImageAIDetectionResult imageAIResult, MediaAI mediaAi)
        {
            var tags = new List<MediaAITag>();
            var objects = new List<MediaAIObject>();

            foreach (ImageAIDetectionItem? item in imageAIResult.Items)
            {

                switch (item.Type)
                {
                    case "tag":
                        tags.Add(new MediaAITag
                        {
                            Name = item.Name,
                            Confidence = item.Probability,
                            Source = AISource.ImageAI
                        });
                        break;
                    case "object":
                        objects.Add(new MediaAIObject
                        {
                            Name = item.Name,
                            Confidence = item.Probability,
                            Source = AISource.ImageAI,
                            Box = item.Box
                        });
                        break;
                }
            }

            mediaAi.Tags = tags;
            mediaAi.Objects = objects;
        }

        private async Task<MediaAI> SaveMediaAI(MediaAI aiData, CancellationToken cancellationToken)
        {
            await _mediaStore.UpdateAISummaryAsync(aiData.MediaId, BuildSummary(aiData), cancellationToken);

            return await _mediaStore.MediaAI.SaveAsync(aiData, cancellationToken);
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

        private async Task<MediaAI> AnalyseMediaAsync(
            Media media,
            ICloudAIMediaAnalyser analyser,
            CancellationToken cancellationToken)
        {
            Log.Information("Analyse media {Id} using {Analyzer}", media.Id, analyser.Source);
            MediaAI aiData = new();

            try
            {
                MediaBlobData? request = _mediaService.GetBlobRequest(media, MediaFileType.Original);

                using Stream imageStream = _mediaStore.Blob.GetStreamAsync(request);
                using Stream stream = await RemoveExifDataAsync(imageStream, cancellationToken);



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
