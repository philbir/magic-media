using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Mutation")]
    public class UserMutations
    {
        private readonly IUserService _userService;

        public UserMutations(IUserService userService)
        {
            _userService = userService;
        }   

        [GraphQLName("User_CreateFromPerson")]
        public async Task<UpdateUserPayload> CreateUserFromPersonAsync(
            CreateUserFromPersonRequest input,
            CancellationToken cancellationToken)
        {
            User user = await _userService.CreateFromPersonAsync(input, cancellationToken);

            return new UpdateUserPayload(user);
        }

        public class UpdateUserPayload: Payload
        {
            public UpdateUserPayload(User? user)
            {
                User = user;
            }

            public User? User { get; }

            public UpdateUserPayload(IReadOnlyList<UserError>? errors = null)
                : base(errors)
            {
            }
        }
    }
}
