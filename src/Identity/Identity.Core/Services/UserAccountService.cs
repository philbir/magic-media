using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Identity.Data;

namespace MagicMedia.Identity.Services;

public class UserAccountService : IUserAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFactory _userFactory;
    private readonly UserAccountOptions _options;

    public UserAccountService(
        IUserRepository userRepository,
        IUserFactory userFactory,
        UserAccountOptions options)
    {
        _userRepository = userRepository;
        _userFactory = userFactory;
        _options = options;
    }

    public async Task<AuthenticateUserResult> AuthenticateExternalUserAsync(
        AuthenticateExternalUserRequest request,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.TryGetUserByProvider(
            request.Provider,
            request.UserIdentifier,
            cancellationToken);

        if (user != null)
        {
            return new AuthenticateUserResult(true)
            {
                User = user
            };
        }
        else
        {
            if (_options.AutoProvisionUser)
            {
                user = _userFactory.CreateFromExternalLogin(
                    request.Provider,
                    request.UserIdentifier,
                    request.Claims);

                user.LastLogin = DateTime.UtcNow;
                await _userRepository.AddAsync(user, cancellationToken);

                return new AuthenticateUserResult(true)
                {
                    User = user
                };
            }
        }

        return new AuthenticateUserResult(false);
    }
}
