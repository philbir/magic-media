namespace MagicMedia.SamsungTv;

public interface ISamsungTvArtModeClient
{
    Task<IReadOnlyList<SamsungTvMedia>> GetAllMediaAsync(CancellationToken cancellationToken);

    Task<string> UploadAsync(
        byte[] data,
        string filename,
        string? matte,
        CancellationToken cancellationToken);

    Task SelectImageAsync(string id, CancellationToken cancellationToken);
    Task ChangeMatteAsync(string id, string matte, CancellationToken cancellationToken);
    Task<byte[]?> GetPreviewAsync(string id, CancellationToken cancellationToken);
    Task<bool> IsOnlineAsync(CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
    Task<SamsungTvFeatures> GetFeaturesAsync(CancellationToken cancellationToken);
    Task SetFilterAsync(string id, string filter, CancellationToken cancellationToken);
}

public interface ISamsungTvClientFactory
{
    ISamsungTvArtModeClient Create(string name);
    IEnumerable<SamsungTvDevice> GetDevices();
}
