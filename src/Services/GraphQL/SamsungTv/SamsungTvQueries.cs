using MagicMedia.SamsungTv;

namespace MagicMedia.GraphQL.SamsungTv;

[ExtendObjectType(RootTypes.Query)]
public class SamsungTvQueries
{
    public async Task<IReadOnlyList<SamsungTvMedia>> GetSamsungTvMediaAsync(
        [Service] ISamsungTvClientFactory clientFactory,
        string device,
        CancellationToken cancellationToken)
    {
        ISamsungTvArtModeClient client = clientFactory.Create(device);
        return await client.GetAllMediaAsync(cancellationToken);
    }

    public IReadOnlyList<SamsungTvDevice> GetSamsungTvDevices(
        [Service] ISamsungTvClientFactory clientFactory)
    {
        return clientFactory.GetDevices().ToList();
    }

    public async Task<SamsungTvFeatures> GetSamsungTvFeaturesAsync(
        [Service] ISamsungTvClientFactory clientFactory,
        string device,
        CancellationToken cancellationToken)
    {
        ISamsungTvArtModeClient client = clientFactory.Create(device);
        return await client.GetFeaturesAsync(cancellationToken);
    }
}
