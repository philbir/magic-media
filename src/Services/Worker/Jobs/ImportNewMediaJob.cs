using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MagicMedia.Processing;
using MagicMedia.Telemetry;
using OpenTelemetry.Trace;
using Quartz;
using Microsoft.Extensions.Logging;

namespace MagicMedia.Jobs;

public class ImportNewMediaJob : IJob
{
    private readonly IMediaSourceScanner _sourceScanner;
    private readonly IMediaSourcePreConverter _preConverter;
    private readonly ILogger<ImportNewMediaJob> _logger;

    public ImportNewMediaJob(
        IMediaSourceScanner sourceScanner,
        IMediaSourcePreConverter preConverter,
        ILogger<ImportNewMediaJob> logger)
    {
        _sourceScanner = sourceScanner;
        _preConverter = preConverter;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.ExecuteImportNewMediaJob();
        using Activity? activity = Tracing.Source.StartRootActivity(
            "Execute ImportNewMedia job");
        try
        {
            await _preConverter.PreConvertAsync(context.CancellationToken);
            await _sourceScanner.ScanAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            activity.RecordException(ex);
        }
    }
}

public static partial class ImportNewMediaJobLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Execute ImportNewMedia job")]
    public static partial void ExecuteImportNewMediaJob(this ILogger logger);
}
