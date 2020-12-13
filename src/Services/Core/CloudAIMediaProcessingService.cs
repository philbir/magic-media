using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using MagicMedia.Extensions;
using MagicMedia.Store;
using MetadataExtractor;
using Serilog;
using SixLabors.ImageSharp;

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

        public async Task AnalyseMediaAsync(Media media, CancellationToken cancellationToken)
        {
            foreach (ICloudAIMediaAnalyser imageAnalyzer in _analysers)
            {
                MediaAI aiData = await AnalyseMediaAsync(media, imageAnalyzer, cancellationToken);

                await SaveMediaAI(aiData, cancellationToken);
            }
        }

        private async Task SaveMediaAI(MediaAI aiData, CancellationToken cancellationToken)
        {
            MediaAI? existing = await _mediaStore.MediaAI.GetByMediaIdAsync(
                aiData.MediaId,
                cancellationToken);

            if (existing == null)
            {
                await _mediaStore.MediaAI.SaveAsync(aiData, cancellationToken);
            }
            else
            {
                var tags = new List<MediaAITag>(existing.Tags);
                var objects = new List<MediaAIObject>(existing.Objects);
                var sources = new List<MediaAISourceInfo>(existing.SourceInfo?? new List<MediaAISourceInfo>());

                tags.AddRange(aiData.Tags);
                objects.AddRange(aiData.Objects);
                sources.AddRange(aiData.SourceInfo);

                if (aiData.Colors != null)
                {
                    existing.Colors = aiData.Colors;
                }

                if (aiData.Caption != null)
                {
                    existing.Caption = aiData.Caption;
                }

                await _mediaStore.MediaAI.SaveAsync(existing, cancellationToken);
            }
        }

        public async Task ProcessNewBySourceAsync(
            AISource source,
            CancellationToken cancellationToken)
        {
            IEnumerable<MediaAI> result = await _mediaStore.MediaAI.GetWithoutSourceInfoAsync(
                source,
                1000,
                cancellationToken);

            foreach (MediaAI mediaAI in result)
            {
                //Make sure no persons are on the image
                if (mediaAI.Objects.Count(x => x.Name == "person") > 0)
                {
                    continue;
                }

                Media media = await _mediaService.GetByIdAsync(mediaAI.MediaId, cancellationToken);

                ICloudAIMediaAnalyser analyser = _analysers.Single(x => x.Source == source);

                MediaAI aiData = await AnalyseMediaAsync(media, analyser, cancellationToken);

                await SaveMediaAI(aiData, cancellationToken);
            }
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

            MediaAI aiData = await analyser.AnalyseImageAsync(
                    stream,
                    cancellationToken);

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

            await image.SaveAsJpegAsync(ms, cancellationToken);
            ms.Position = 0;
            return ms;
        }
    }
}
