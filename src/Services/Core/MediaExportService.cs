using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using MagicMedia.Store;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia;

public class MediaExportService : IMediaExportService
{
    private readonly IMediaService _mediaService;
    private readonly IMediaStore _mediaStore;
    private readonly FileSystemStoreOptions _fileSystemStoreOptions;

    public MediaExportService(
        IMediaService mediaService,
        IMediaStore mediaStore,
        FileSystemStoreOptions fileSystemStoreOptions)
    {
        _mediaService = mediaService;
        _mediaStore = mediaStore;
        _fileSystemStoreOptions = fileSystemStoreOptions;
    }

    public async Task<MediaExportResult> ExportAsync(
        Guid id,
        Guid? profileId,
        CancellationToken cancellationToken)
    {
        // Take default profile
        var profile = new MediaExportProfile()
        {
            Location = new ExportLocation { Path = "samsung-frame" },
            Size = new ImageSize { Width = 3840, Height = 2160 }
        };

        if (profileId.HasValue)
        {
            // TODO: Load profile from database
        }

        Media media = await _mediaService.GetByIdAsync(id, cancellationToken);

        if (media.MediaType == MediaType.Image)
        {
            Stream mediaStream = _mediaService.GetMediaStream(media);

            using Image image = await Image.LoadAsync(mediaStream, cancellationToken);

            if (profile.Size is { })
            {
                image.Mutate(x => x.Resize( new ResizeOptions
                {
                    Size = new Size(profile.Size.Width, profile.Size.Height),
                    Mode = ResizeMode.Crop,
                }));
            }

            var folder = Path.Combine(_fileSystemStoreOptions.RootDirectory, "export", profile.Location.Path);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filename = CreateFilename(media);
            var exportPath = Path.Combine(folder, filename);

            await image.SaveAsync(exportPath, cancellationToken);

            return new MediaExportResult { Path = exportPath };
        }

        return new MediaExportResult();

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

        sb.Append(media.Filename);

        return RemoveIllegalChars(sb.ToString());
    }

    private string RemoveIllegalChars(string filename)
    {
        return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
    }

}
