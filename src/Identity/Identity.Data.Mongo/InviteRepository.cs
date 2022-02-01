using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Identity.Data.Mongo;

public class InviteRepository : IInviteRepository
{
    private readonly IIdentityDbContext _loginDbContext;

    public InviteRepository(IIdentityDbContext loginDbContext)
    {
        _loginDbContext = loginDbContext;
    }

    public async Task<Invite> AddAsync(Invite invite, CancellationToken cancellationToken)
    {
        await _loginDbContext.Invites.InsertOneAsync(
            invite,
            options: null,
            cancellationToken);

        return invite;
    }

    public async Task<Invite> UpdateAsync(Invite invite, CancellationToken cancellationToken)
    {
        await _loginDbContext.Invites.ReplaceOneAsync(
            x => x.Id == invite.Id,
            invite,
            new ReplaceOptions(),
            cancellationToken);

        return invite;
    }

    public async Task<Invite> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _loginDbContext.Invites.AsQueryable()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Invite> TryGetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await _loginDbContext.Invites.AsQueryable()
            .Where(x => x.Code == code)
            .FirstOrDefaultAsync();
    }
}
