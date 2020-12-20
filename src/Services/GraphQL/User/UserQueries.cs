using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MagicMedia.Authorization;
using MagicMedia.Search;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class UserQueries
    {
        private readonly IUserService _userService;
        private readonly IUserContextFactory _userContextFactory;

        public UserQueries(
            IUserService userService,
            IUserContextFactory userContextFactory)
        {
            _userService = userService;
            _userContextFactory = userContextFactory;
        }

        public async Task<User> GetMeAsync(CancellationToken cancellationToken)
        {
            IUserContext context = await _userContextFactory.CreateAsync(cancellationToken);
            User user = await _userService.TryGetByIdAsync(context.UserId.Value, cancellationToken);

            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            IUserContext context = await _userContextFactory.CreateAsync(cancellationToken);

            if (context.HasPermission(Permissions.User.Manage))
            {
                return await _userService.GetAllAsync(cancellationToken);
            }
            else
            {
                return new List<User>();
            }
        }

        [Authorize(Apply = ApplyPolicy.BeforeResolver, Policy = AuthorizationPolicies.Names.ManageUsers)]
        public async Task<User> GetUserAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _userService.TryGetByIdAsync(id, cancellationToken);
        }

        [Authorize(Apply = ApplyPolicy.BeforeResolver, Policy = AuthorizationPolicies.Names.ManageUsers)]
        public async Task<SearchResult<User>> SearchAsync(SearchUserRequest input, CancellationToken cancellationToken)
        {
            return await _userService.SearchAsync(input, cancellationToken);
        }
    }
}
