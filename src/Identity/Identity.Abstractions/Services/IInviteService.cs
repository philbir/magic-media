using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Identity.Data;

namespace MagicMedia.Identity.Services
{
    public interface IInviteService
    {
        Task<Invite> CreateInviteAsync(CreateInviteRequest request, CancellationToken cancellationToken);
        Task SetUsedAsync(Guid id, CancellationToken cancellationToken);
        Task<Invite?> TryGetByCodeAsync(string code, CancellationToken cancellationToken);
    }

    public record CreateInviteRequest(Guid UserId, string Name, string Email);

}
