using System.Threading.Tasks;
using MagicMedia.Identity.Data;
using MagicMedia.Identity.Services;
using MagicMedia.Messaging;
using MassTransit;

namespace MagicMedia.Identity.Messaging;

public class InviteUserRequestedConsumer : IConsumer<InviteUserRequestedMessage>
{
    private readonly IInviteService _inviteService;

    public InviteUserRequestedConsumer(IInviteService inviteService)
    {
        _inviteService = inviteService;
    }

    public async Task Consume(ConsumeContext<InviteUserRequestedMessage> context)
    {
        var request = new CreateInviteRequest(
            context.Message.UserId,
            context.Message.Name,
            context.Message.Email);

        Invite invite = await _inviteService.CreateInviteAsync(
            request,
            context.CancellationToken);

        var createdMessage = new InviteUserCreatedMessage(
            invite.UserId,
            invite.Name,
            invite.Email,
            invite.Code);

        await context.Publish(createdMessage, context.CancellationToken);
    }
}
