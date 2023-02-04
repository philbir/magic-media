using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Face;

public interface IFaceDetectionService
{
    Task<BuildFaceModelResult> BuildModelAsync(
        IEnumerable<PersonEncodingData> encodings,
        CancellationToken cancellationToken);

    Task<IEnumerable<FaceDetectionResponse>> DetectFacesAsync(
        Stream stream,
        CancellationToken cancellationToken);

    Task<Guid?> PredictPersonAsync(
        PredictPersonRequest request,
        CancellationToken cancellationToken);
}
