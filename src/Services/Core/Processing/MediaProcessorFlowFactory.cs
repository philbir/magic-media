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
                case "ImportImage":
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
                case "ImportImageNoFace":
                    return new MediaProcessorFlow(_taskFactory, new[]
                    {
                        MediaProcessorTaskNames.AutoOrient,
                        MediaProcessorTaskNames.ExtractMetadata,
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
