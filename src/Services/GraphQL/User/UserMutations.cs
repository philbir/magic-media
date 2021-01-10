using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MagicMedia.Authorization;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Mutation")]
    [Authorize(Apply = ApplyPolicy.BeforeResolver, Policy = AuthorizationPolicies.Names.UserEdit)]
    public class UserMutations
    {
        private readonly IUserService _userService;
        private readonly IAlbumService _albumService;

        public UserMutations(
            IUserService userService,
            IAlbumService albumService)
        {
            _userService = userService;
            _albumService = albumService;
        }   

        [GraphQLName("User_CreateFromPerson")]
        public async Task<UpdateUserPayload> CreateUserFromPersonAsync(
            CreateUserFromPersonRequest input,
            CancellationToken cancellationToken)
        {
            User user = await _userService.CreateFromPersonAsync(input, cancellationToken);

            return new UpdateUserPayload(user);
        }

        [GraphQLName("User_CreateInvite")]
        public async Task<UpdateUserPayload> CreateIniteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            User user = await _userService.CreateInviteAsync(id, cancellationToken);

            return new UpdateUserPayload(user);
        }


        [GraphQLName("User_SaveSharedAlbums")]
        public async Task<UpdateUserPayload> SaveSharedAlbumsAsync(
            SaveUserSharedAlbumsRequest input,
            CancellationToken cancellationToken)
        {
            await _albumService.SaveUserSharedAlbumsAsync(input, cancellationToken);

            _userService.InvalidateUserCacheAsync(input.UserId);

            User user = await _userService.GetByIdAsync(input.UserId, cancellationToken);

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
