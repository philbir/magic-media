using System.Diagnostics;

namespace MagicMedia.Telemetry;

public static class ActivitySourceExtensions
{
    public static Activity? StartRootActivity(
        this ActivitySource source,
        string name,
        ActivityKind kind = ActivityKind.Internal)
    {
        Activity.Current = null;
        Activity? activity = source.StartActivity(name, kind);

        return activity;
    }
}


