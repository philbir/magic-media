namespace MagicMedia.GraphQL;

[ExtendObjectType(RootTypes.Query)]
public class MediaExportProfileQueries
{
    public Task<IEnumerable<MediaExportProfile>> GetMediaExportProfilesAsync(
        [Service] IMediaExportProfileService service,
        CancellationToken cancellationToken)
    {
        return service.GetAllAsync(cancellationToken);
    }
}
