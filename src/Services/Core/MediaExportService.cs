using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;

namespace MagicMedia;

public class MediaExportService : IMediaExportService
{
    private readonly IMediaExportProfileService _profileService;
    private readonly IMediaTransformService _transformService;
    private readonly IEnumerable<IDestinationExporter> _destinationExporters;
    private readonly FileSystemStoreOptions _fileSystemStoreOptions;

    public MediaExportService(
        IMediaExportProfileService profileService,
        IMediaTransformService transformService,
        IEnumerable<IDestinationExporter> destinationExporters)
    {
        _profileService = profileService;
        _transformService = transformService;
        _destinationExporters = destinationExporters;
    }

    public async Task<MediaExportResult> ExportAsync(
        Guid id,
        MediaExportOptions options,
        CancellationToken cancellationToken)
    {
        MediaExportProfile profile = await _profileService.GetProfileOrDefault(options.ProfileId, cancellationToken);

        TransformedMedia transformed = await _transformService.TransformAsync(id, profile.Transform, cancellationToken);

        var exports = new List<string>();

        foreach (ExportDestination destination in profile.Destinations)
        {
            IDestinationExporter exporter = _destinationExporters
                .Single(x => x.CanHandleType == destination.Type);

            var path = await exporter.ExportAsync(transformed, destination, options, cancellationToken);
            exports.Add(path);
        }

        return new MediaExportResult { Path = string.Join(Environment.NewLine, exports) };
    }
}
