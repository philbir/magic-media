using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Identity.Data;

namespace MagicMedia.Identity.Services;

public interface IInviteService
{
    Task CreateAccountAsync(Guid id, CancellationToken cancellationToken);
    Task<Invite> CreateInviteAsync(CreateInviteRequest request, CancellationToken cancellationToken);
    Task<Invite> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task SetUsedAsync(Guid id, CancellationToken cancellationToken);
    Task<Invite?> TryGetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<Invite> UpdateAsync(Invite invite, CancellationToken cancellationToken);
}
