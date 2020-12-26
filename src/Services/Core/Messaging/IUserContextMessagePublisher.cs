using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Messaging
{
    public interface IUserContextMessagePublisher
    {
        Task PublishAsync(IUserContextMessage message, CancellationToken cancellationToken);
        Task PublishAsync(IUserContextMessage message, IUserContext userContext, CancellationToken cancellationToken);
    }
}