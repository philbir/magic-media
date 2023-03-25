using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public interface IMediaEditorService
{
    Task<Media> SaveEditedImageAsync(
        Guid id,
        string type,
        Stream stream,
        CancellationToken cancellationToken);
}
