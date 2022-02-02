using System.Diagnostics;
using System.Threading.Tasks;
using MagicMedia.Processing;
using Quartz;
using Serilog;

namespace MagicMedia.Jobs;

public class ImportNewMediaJob : IJob
{
    private readonly IMediaSourceScanner _sourceScanner;

    private static ActivitySource Activity = new ActivitySource("ImportNewMediaJob");

    public ImportNewMediaJob(IMediaSourceScanner sourceScanner)
    {
        _sourceScanner = sourceScanner;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Activity.StartActivity("Execute");

        await _sourceScanner.ScanAsync(context.CancellationToken);
    }
}
