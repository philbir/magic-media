using MagicMedia.SamsungTv;

namespace MagicMedia.GraphQL.SamsungTv;

public class SamsungTvDeviceType : ObjectType<SamsungTvDevice>
{
    protected override void Configure(IObjectTypeDescriptor<SamsungTvDevice> descriptor)
    {
        descriptor
            .Field("online")
            .ResolveWith<Resolvers>(x => x.GetOnlineAsync(default!, default!, default!));
    }

    class Resolvers
    {
        internal Task<bool> GetOnlineAsync(
            [Parent] SamsungTvDevice device,
            [Service] ISamsungTvClientFactory clientFactory,
            CancellationToken cancellationToken)
        {
            return clientFactory.Create(device.Name).IsOnlineAsync(cancellationToken);
        }
    }
}
