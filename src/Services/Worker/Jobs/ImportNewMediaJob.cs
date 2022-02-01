using System.Threading.Tasks;
using MagicMedia.Processing;
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
        //Log.Information("Executing ImportNewMedia job");
        await _sourceScanner.ScanAsync(context.CancellationToken);
    }
}
