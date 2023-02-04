using System;
using System.IO;

namespace MagicMedia;

public class MediaDownload : IDisposable
{
    public MediaDownload(Stream stream, string filename)
    {
        Stream = stream;
        Filename = filename;
    }

    public Stream Stream { get; }
    public string Filename { get; }

    public void Dispose()
    {
        Stream?.Dispose();
    }
}
