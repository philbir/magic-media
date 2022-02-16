using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MagicMedia.Processing;
using MagicMedia.Telemetry;
using OpenTelemetry.Trace;
using Quartz;
using Serilog;

namespace MagicMedia.Jobs;

public class ImportNewMediaJob : IJob
{
    private readonly IMediaSourceScanner _sourceScanner;

    public ImportNewMediaJob(IMediaSourceScanner sourceScanner)
    {
        _sourceScanner = sourceScanner;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using Activity? activity = Tracing.Source.StartRootActivity(
            "Execute ImportNewMedia job");

        try
        {
            await _sourceScanner.ScanAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            activity.RecordException(ex);
        }
    }
}
