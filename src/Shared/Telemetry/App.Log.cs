using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MagicMedia;

public static partial class App
{
    internal static ILoggerFactory? LoggerFactory { get; }

    private static readonly Lazy<ILogger> _log =
        new(() => LoggerFactory!.CreateLogger("MagicMedia"));

    public static ILogger Log => LoggerFactory == null ? NullLogger.Instance : _log.Value;
}

public static partial class App
{
    private static readonly Lazy<ActivitySource> _activitySource = new(() =>
        new ActivitySource(GetName(), GetVersion()));

    /// <summary>
    /// Default <see cref="ActivitySource"/> for the running application
    /// </summary>
    public static ActivitySource ActivitySource => _activitySource.Value;
}

public static partial class App
{

    private static string? _version;
    private static Assembly? _entryAssembly;

    private static string GetVersion()
    {
        if (string.IsNullOrEmpty(_version))
        {
            _version = GetEntryAssembly()?
                .GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "unknown";
        }

        return _version;
    }

    private static string GetName()
    {
        return GetEntryAssembly()?.GetName().Name ?? "unknown";
    }

    private static Assembly? GetEntryAssembly()
    {
        if (_entryAssembly == null)
        {
            _entryAssembly = Assembly.GetEntryAssembly();
        }

        return _entryAssembly;
    }
}
