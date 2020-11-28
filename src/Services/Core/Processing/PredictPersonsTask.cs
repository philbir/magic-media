using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.Store;

namespace MagicMedia.Processing
{
    public class PredictPersonsTask : IMediaProcessorTask
    {
        private readonly IFaceDetectionService _faceDetectionService;

        public PredictPersonsTask(IFaceDetectionService faceDetectionService)
        {
            _faceDetectionService = faceDetectionService;
        }

        public string Name => MediaProcessorTaskNames.PredictPersons;

        public async Task ExecuteAsync(
            MediaProcessorContext context,
            CancellationToken cancellationToken)
        {
            if (context.FaceData != null)
            {
                double distance = 0.4;

                foreach (FaceData face in context.FaceData)
                {
                    try
                    {
                        Guid? personId = await _faceDetectionService.PredictPersonAsync(
                            new PredictPersonRequest
                            {
                                Distance = distance,
                                Encoding = face.Encoding
                            }, cancellationToken);

                        if (personId.HasValue)
                        {
                            face.PersonId = personId;
                            face.DistanceThreshold = distance;
                            face.RecognitionType = FaceRecognitionType.Computer;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }
        }
    }
}
