using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;

namespace MagicMedia.Operations
{
    public interface IRescanFacesHandler
    {
        Task ExecuteAsync(RescanFacesMessage message, CancellationToken cancellationToken);
    }
}