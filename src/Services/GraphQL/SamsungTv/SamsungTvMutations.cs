using MagicMedia.SamsungTv;

namespace MagicMedia.GraphQL.SamsungTv;

[ExtendObjectType(RootTypes.Mutation)]
public class SamsungTvMutations
{
    public async Task<SamsungTvMediaPayload> DeleteSamsungTvMediaAsync(
        [Service] ISamsungTvClientFactory clientFactory,
        DeleteSamsungTvMediaInput input,
        CancellationToken cancellationToken)
    {
        ISamsungTvArtModeClient client = clientFactory.Create(input.Device);
        await client.DeleteAsync(input.Id, cancellationToken);

        return new SamsungTvMediaPayload(input.Id);
    }

    public async  Task<SamsungTvMediaPayload> SelectSamsungTvMediaAsync(
        [Service] ISamsungTvClientFactory clientFactory,
        SelectSamsungTvMediaInput input,
        CancellationToken cancellationToken)
    {
        ISamsungTvArtModeClient client = clientFactory.Create(input.Device);
        await client.SelectImageAsync(input.Id, cancellationToken);

        return new SamsungTvMediaPayload(input.Id);
    }

    public async Task<SamsungTvMediaPayload> ChangeSamsungTvMatteAsync(
        [Service] ISamsungTvClientFactory clientFactory,
        ChangeSamsungTvMatteInput input,
        CancellationToken cancellationToken)
    {
        ISamsungTvArtModeClient client = clientFactory.Create(input.Device);
        await client.ChangeMatteAsync(input.Id, input.Matte, cancellationToken);

        return new SamsungTvMediaPayload(input.Id);
    }

    public async Task<SamsungTvMediaPayload> SetSamsungTvFilterAsync(
        [Service] ISamsungTvClientFactory clientFactory,
        SetSamsungTvFilterInput input,
        CancellationToken cancellationToken)
    {
        ISamsungTvArtModeClient client = clientFactory.Create(input.Device);
        await client.SetFilterAsync(input.Id, input.Filter, cancellationToken);

        return new SamsungTvMediaPayload(input.Id);
    }
}

public record SelectSamsungTvMediaInput(
    string Device,
    string Id);

public record DeleteSamsungTvMediaInput(
    string Device,
    string Id);

public record ChangeSamsungTvMatteInput(
    string Device,
    string Id,
    string Matte);

public record SetSamsungTvFilterInput(
    string Device,
    string Id,
    string Filter);

public class SamsungTvMediaPayload
{
    public SamsungTvMediaPayload(string id)
    {
        Id = id;
    }

    public string Id { get; }
}
