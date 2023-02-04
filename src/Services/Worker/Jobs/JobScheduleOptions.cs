using System;

namespace MagicMedia.Jobs;

public class JobScheduleOptions
{
    public string? Name { get; set; }

    public bool Enabled { get; set; }

    public TimeSpan? Interval { get; set; }

    public string? Cron { get; set; }
}
