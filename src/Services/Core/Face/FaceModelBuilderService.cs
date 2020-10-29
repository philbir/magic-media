using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia.Face
{
    public class FaceModelBuilderService : IFaceModelBuilderService
    {
        private readonly IFaceStore _faceStore;
        private readonly IFaceDetectionService _faceDetectionService;

        public FaceModelBuilderService(
            IFaceStore faceStore,
            IFaceDetectionService faceDetectionService)
        {
            _faceStore = faceStore;
            _faceDetectionService = faceDetectionService;
        }

        public async Task<BuildFaceModelResult> BuildModelAsyc(CancellationToken cancellationToken)
        {
            IEnumerable<PersonEncodingData>? encodings = await _faceStore
                .GetPersonEncodingsAsync(cancellationToken);

            BuildFaceModelResult result = await _faceDetectionService.BuildModelAsync(
                encodings,
                cancellationToken);

            return result;
        }
    }
}
