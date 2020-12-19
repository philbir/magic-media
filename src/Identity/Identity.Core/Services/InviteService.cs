using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Identity.Data;
using MagicMedia.Identity.Data.Mongo;

namespace MagicMedia.Identity.Services
{
    public class InviteService : IInviteService
    {
        private readonly IInviteRepository _repository;
        private readonly IUserRepository _userRepository;

        public InviteService(
            IInviteRepository repository,
            IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<Invite> CreateInviteAsync(
            CreateInviteRequest request,
            CancellationToken cancellationToken)
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

        public async Task<Invite> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(id, cancellationToken);
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

        public async Task<Invite> UpdateAsync(Invite invite, CancellationToken cancellationToken)
        {
            invite = await _repository.UpdateAsync(invite, cancellationToken);

            return invite;
        }

        public async Task CreateAccountAsync(Guid id, CancellationToken cancellationToken)
        {
            Invite invite = await GetByIdAsync(id, cancellationToken);
            invite.UsedAt = DateTime.UtcNow;
            User user = CreateUserFromInvite(invite);

            await _userRepository.AddAsync(user, cancellationToken);

            await UpdateAsync(invite, cancellationToken);
        }

        public User CreateUserFromInvite(
            Invite invite)
        {
            return new User
            {
                Id = invite.UserId,
                Name = invite.Name,
                CreatedAt = DateTime.UtcNow,
                Status = UserStatus.Active,
                AuthProviders = new List<UserAuthProvider>
                {
                    new() { Name = invite.Provider, UserIdentifier = invite.ProviderUserId}
                }
            };
        }
    }
}
