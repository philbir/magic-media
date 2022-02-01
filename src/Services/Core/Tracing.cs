using System.Diagnostics;

namespace MagicMedia;

internal static class Tracing
{
    internal static ActivitySource Core = new ActivitySource("MagicMedia.Core", "1.0.0");
}
