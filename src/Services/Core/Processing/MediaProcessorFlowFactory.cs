using System;

namespace MagicMedia.Processing
{
    public class MediaProcessorFlowFactory : IMediaProcessorFlowFactory
    {
        private readonly IMediaProcesserTaskFactory _taskFactory;

        public MediaProcessorFlowFactory(IMediaProcesserTaskFactory taskFactory)
        {
            _taskFactory = taskFactory;
        }

        public IMediaProcessorFlow CreateFlow(string name)
        {
            switch (name)
            {
                case "ImportImageWithFace":
                    return new MediaProcessorFlow(_taskFactory, new[]
                    {
                        MediaProcessorTaskNames.AutoOrient,
                        MediaProcessorTaskNames.ExtractMetadata,
                        MediaProcessorTaskNames.GenerateThumbnails,
                        MediaProcessorTaskNames.BuildFaceData,
                        MediaProcessorTaskNames.PredictPersons,
                        MediaProcessorTaskNames.GenerateWebImage,
                        MediaProcessorTaskNames.SaveMedia,
                    });
                case "ImportImage":
                    return new MediaProcessorFlow(_taskFactory, new[]
                    {
                        MediaProcessorTaskNames.AutoOrient,
                        MediaProcessorTaskNames.ExtractMetadata,
                        MediaProcessorTaskNames.GenerateThumbnails,
                        MediaProcessorTaskNames.GenerateWebImage,
                        MediaProcessorTaskNames.SaveMedia,
                        MediaProcessorTaskNames.CleanUpSource
                    });
                case "ScanFaces":
                    return new MediaProcessorFlow(_taskFactory, new[]
                    {
                        MediaProcessorTaskNames.BuildFaceData,
                        MediaProcessorTaskNames.PredictPersons,
                        MediaProcessorTaskNames.SaveFaces,
                    });
                case "ImportVideo":
                    return new MediaProcessorFlow(_taskFactory, new[]
                    {
                        MediaProcessorTaskNames.ExtractVideoData,
                        MediaProcessorTaskNames.GenerateThumbnails,
                        MediaProcessorTaskNames.GenerateWebImage,
                        MediaProcessorTaskNames.SaveMedia,
                    });
                case "BuildPreviewVideos":
                    return new MediaProcessorFlow(_taskFactory, new[]
                    {
                        MediaProcessorTaskNames.ExtractVideoData,
                        MediaProcessorTaskNames.GenerateThumbnails,
                        MediaProcessorTaskNames.GenerateWebImage,
                        MediaProcessorTaskNames.SaveMedia,
                    });
                default:
                    throw new ArgumentException("Invalid flow", nameof(name));
            }
        }
    }


}
