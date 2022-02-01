using System.IO;

namespace MagicMedia;

public record MediaStream(Stream Stream, string MimeType);
