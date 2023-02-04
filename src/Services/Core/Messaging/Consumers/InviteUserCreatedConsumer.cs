using System.Threading.Tasks;
using MagicMedia.Security;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia.Messaging.Consumers;

public class InviteUserCreatedConsumer : IConsumer<InviteUserCreatedMessage>
{
    private readonly IUserService _userService;

    public InviteUserCreatedConsumer(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<InviteUserCreatedMessage> context)
    {
        User user = await _userService.GetByIdAsync(
            context.Message.UserId,
            context.CancellationToken);

        user.State = UserState.Invited;
        user.InvitationCode = context.Message.Code;

        await _userService.UpdateAsync(user, context.CancellationToken);

        //We could send an email here...
    }
}
