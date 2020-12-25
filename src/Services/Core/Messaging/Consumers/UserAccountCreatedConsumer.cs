using System.Threading.Tasks;
using MagicMedia.Security;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia.Messaging.Consumers
{
    public class UserAccountCreatedConsumer : IConsumer<UserAccountCreatedMessage>
    {
        private readonly IUserService _userService;

        public UserAccountCreatedConsumer(IUserService userService)
        {
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<UserAccountCreatedMessage> context)
        {
            User user = await _userService.GetByIdAsync(
                context.Message.UserId,
                context.CancellationToken);

            user.State = UserState.Active;
            user.InvitationCode = null;

            await _userService.UpdateAsync(user, context.CancellationToken);

            _userService.InvalidateUserCacheAsync(user.Id);
        }
    }
}
