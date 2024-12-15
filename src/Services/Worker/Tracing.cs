using System.Diagnostics;
using System.Reflection;

namespace MagicMedia;

internal static class Tracing
{
    private static readonly AssemblyName AssemblyName
        = typeof(Tracing).Assembly.GetName();

    internal static readonly ActivitySource Source = new ActivitySource(
        AssemblyName.Name,
        AssemblyName.Version.ToString());
}
