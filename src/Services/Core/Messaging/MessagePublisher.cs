using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Security;
using MassTransit;

namespace MagicMedia.Messaging
{
    public class UserContextMessagePublisher : IUserContextMessagePublisher
    {
        private readonly IUserContextFactory _userContextFactory;
        private readonly IBus _bus;

        public UserContextMessagePublisher(
            IUserContextFactory userContextFactory,
            IBus bus)
        {
            _userContextFactory = userContextFactory;
            _bus = bus;
        }

        public async Task PublishAsync(
            IUserContextMessage message,
            CancellationToken cancellationToken)
        {
            IUserContext userContext = await _userContextFactory.CreateAsync(cancellationToken);

            await PublishAsync(message, userContext, cancellationToken);
        }

        public async Task PublishAsync(
            IUserContextMessage message,
            IUserContext userContext,
            CancellationToken cancellationToken)
        {
            message.ClientInfo = userContext.GetClientInfo();
            message.UserId = userContext.UserId.GetValueOrDefault();

            await _bus.Publish(message, cancellationToken);
        }
    }
}
