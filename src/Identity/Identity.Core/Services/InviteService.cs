using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Identity.Data;
using MagicMedia.Identity.Data.Mongo;

namespace MagicMedia.Identity.Services
{
    public class InviteService : IInviteService
    {
        private readonly IInviteRepository _repository;

        public InviteService(IInviteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Invite> CreateInviteAsync(CreateInviteRequest request, CancellationToken cancellationToken)
        {
            var invite = new Invite
            {
                Id = Guid.NewGuid(),
                Code = Password.GenerateRandomPassword(32),
                Email = request.Email,
                Name = request.Name,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,

            };

            await _repository.AddAsync(invite, cancellationToken);

            return invite;
        }

        public async Task<Invite?> TryGetByCodeAsync(string code, CancellationToken cancellationToken)
        {
            Invite? invite = await _repository.TryGetByCodeAsync(code, cancellationToken);

            if ( invite != null)
            {
                if (invite.UsedAt.HasValue)
                {
                    return null;
                }
            }  

            return invite;
        }


        public async Task SetUsedAsync(Guid id, CancellationToken cancellationToken)
        {
            Invite invite = await _repository.GetByIdAsync(id, cancellationToken);

            invite.UsedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(invite, cancellationToken);
        }
    }

}
