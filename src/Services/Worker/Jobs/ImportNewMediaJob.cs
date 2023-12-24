using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MagicMedia.Processing;
using MagicMedia.Telemetry;
using OpenTelemetry.Trace;
using Quartz;

namespace MagicMedia.Jobs;

public class ImportNewMediaJob : IJob
{
    private readonly IMediaSourceScanner _sourceScanner;
    private readonly IMediaSourcePreConverter _preConverter;

    public ImportNewMediaJob(
        IMediaSourceScanner sourceScanner,
        IMediaSourcePreConverter preConverter)
    {
        _sourceScanner = sourceScanner;
        _preConverter = preConverter;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using Activity? activity = App.ActivitySource.StartRootActivity(
            "Execute ImportNewMedia job");

        try
        {
            await _preConverter.ProConvertAsync(context.CancellationToken);
            await _sourceScanner.ScanAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            activity.RecordException(ex);
        }
    }
}
