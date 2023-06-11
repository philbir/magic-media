using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using MagicMedia.Store;

namespace MagicMedia;

public class FileSystemDestinationExporter : IDestinationExporter
{
    private readonly FileSystemStoreOptions _fileSystemStoreOptions;

    public FileSystemDestinationExporter(FileSystemStoreOptions fileSystemStoreOptions)
    {
        _fileSystemStoreOptions = fileSystemStoreOptions;
    }

    public ExportDestinationType CanHandleType => ExportDestinationType.FileSystem;

    public async Task<string> ExportAsync(
        TransformedMedia media,
        ExportDestination destination,
        MediaExportOptions options,
        CancellationToken cancellationToken)
    {
        var exportPath = GetFileSystemExportPath(media, destination, options);
        await File.WriteAllBytesAsync(exportPath, media.Data, cancellationToken);

        return exportPath;
    }

    private string GetFileSystemExportPath(
        TransformedMedia media,
        ExportDestination destination,
        MediaExportOptions options)
    {
        var folder = Path.Combine(_fileSystemStoreOptions.RootDirectory, "export", destination.Name);

        if (!string.IsNullOrEmpty(options.Path))
        {
            folder = Path.Combine(folder, options.Path);
        }

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
