using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Identity.Data.Mongo
{
    public interface IInviteRepository
    {
        Task<Invite> AddAsync(Invite invite, CancellationToken cancellationToken);
        Task<Invite> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Invite> TryGetByCodeAsync(string code, CancellationToken cancellationToken);
        Task<Invite> UpdateAsync(Invite invite, CancellationToken cancellationToken);
    }
}