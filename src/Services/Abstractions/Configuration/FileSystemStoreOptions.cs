using System.Collections.Generic;

namespace MagicMedia.Configuration;

public class FileSystemStoreOptions
{
    public string? RootDirectory { get; set; }

    public Dictionary<MediaBlobType, string>? BlobTypeMap { get; set; }
}
