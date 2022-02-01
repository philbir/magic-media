using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Face;

public interface IFaceModelBuilderService
{
    Task<BuildFaceModelResult> BuildModelAsyc(CancellationToken cancellationToken);
}
