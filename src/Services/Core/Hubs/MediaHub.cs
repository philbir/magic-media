using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MagicMedia.Hubs;

public class MediaHub : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    //public async Task MoveMediaCompleted(
    //    MoveMediaCompletedMessage message,
    //    CancellationToken cancellationToken)
    //{
    //    await Clients.All.SendAsync(
    //        "moveMediaCompleted",
    //        message,
    //        cancellationToken);
    //}
}
