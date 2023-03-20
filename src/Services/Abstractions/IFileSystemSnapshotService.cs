using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia;

public interface IFileSystemSnapshotService
{
    Task CreateAsync(CancellationToken cancellationToken);
    Task<FileSystemSnapshot> LoadAsync(CancellationToken cancellationToken);
}


public record FileSystemSnapshot
{
    public DateTime CreatedAt { get; init; }

    public List<MediaFileEntry> Entries { get; init; }
}

public record MediaFileEntry(string Name, string Folder);
