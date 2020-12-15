using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class UserQueries
    {
        private readonly IUserService _userService;

        public UserQueries(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _userService.GetAllAsync(cancellationToken);
        }
    }
}
