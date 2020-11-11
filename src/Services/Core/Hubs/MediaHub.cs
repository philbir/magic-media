using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using Microsoft.AspNetCore.SignalR;

namespace MagicMedia.Hubs
{
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
}
