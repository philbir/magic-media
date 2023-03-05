using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using MagicMedia.Security;
using MagicMedia.Store;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia;

public class MediaExportService : IMediaExportService
{
    private readonly IMediaService _mediaService;
    private readonly IMediaExportProfileService _profileService;
    private readonly IMediaTransformService _transformService;
    private readonly FileSystemStoreOptions _fileSystemStoreOptions;

    public MediaExportService(
        IMediaService mediaService,
        IMediaExportProfileService profileService,
        IMediaTransformService transformService,
        FileSystemStoreOptions fileSystemStoreOptions)
    {
        _mediaService = mediaService;
        _profileService = profileService;
        _transformService = transformService;
        _fileSystemStoreOptions = fileSystemStoreOptions;
    }

    public async Task<MediaExportResult> ExportAsync(
        IEnumerable<Guid> ids,
        MediaExportProfile profile,
        CancellationToken cancellationToken)
    {
        return null;
    }


    public async Task<MediaExportResult> ExportAsync(
        Guid id,
        Guid? profileId,
        CancellationToken cancellationToken)
    {
        MediaExportProfile profile = await _profileService.GetProfileOrDefault(profileId, cancellationToken);

        TransformedMedia transformed = await _transformService.TransformAsync(id, profile.Transform, cancellationToken);

        if (profile.Location.Type == LocationType.FileSystem)
        {
            var exportPath = GetFileSystemExportPath(transformed, profile.Location);
            await File.WriteAllBytesAsync(exportPath, transformed.Data, cancellationToken);

            return new MediaExportResult { Path = exportPath };
        }

        throw new NotSupportedException("Only FileSystem export supported at the moment");
    }

    private string GetFileSystemExportPath(TransformedMedia media, ExportLocation location)
    {
        var folder = Path.Combine(_fileSystemStoreOptions.RootDirectory, "export", location.Path);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var filename = CreateFilename(media.Media) + $".{media.Format.ToLower()}";
        var exportPath = Path.Combine(folder, filename);

        return exportPath;
    }


    private string CreateFilename(Media media)
    {
        var sb = new StringBuilder();
        if (media.Folder != null)
        {
            var folders = media.Folder.Split('/', StringSplitOptions.RemoveEmptyEntries);

            sb.Append(string.Join("_", folders));
            sb.Append('_');
        }

        sb.Append(Path.GetFileNameWithoutExtension(media.Filename));

        return RemoveIllegalChars(sb.ToString());
    }

    private string RemoveIllegalChars(string filename)
    {
        return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
    }
}
