namespace MagicMedia.SamsungTv;

public class SamsungTvExporter : IDestinationExporter
{
    private readonly ISamsungTvClientFactory _tvClientFactory;

    public SamsungTvExporter(ISamsungTvClientFactory tvClientFactory)
    {
        _tvClientFactory = tvClientFactory;
    }

    public ExportDestinationType CanHandleType => ExportDestinationType.SamsungTvArt;

    public async Task<string> ExportAsync(
        TransformedMedia media,
        ExportDestination destination,
        MediaExportOptions options,
        CancellationToken cancellationToken)
    {
        var matte = destination.Options.FirstOrDefault(x => x.Name == "Matte")?.Value ?? "none";

        ISamsungTvArtModeClient client = _tvClientFactory.Create(destination.Name);

        var id = await client.UploadAsync(
            media.Data,
            media.Media.Filename,
            matte,
            cancellationToken);

        var select = destination.Options.FirstOrDefault(x => x.Name == "Select")?.Value == "true";

        if (matte != "none")
        {
            await client.ChangeMatteAsync(id, matte, cancellationToken);
        }

        if (select)
        {
            await client.SelectImageAsync(id, cancellationToken);
        }

        return $"{destination.Name}/{id}";
    }
}
