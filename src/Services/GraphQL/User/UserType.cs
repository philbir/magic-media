using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor
            .Field("person")
                .ResolveWith<UserResolvers>(x => x.GetPersonAsync(default!, default!));
        }
    }
}
