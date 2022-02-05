using System.Diagnostics;
using System.Threading.Tasks;
using Quartz;

namespace MagicMedia.Jobs;

public class UpdateAllAlbumSummaryJob : IJob
{
    private readonly IAlbumSummaryService _albumSummaryService;

    public UpdateAllAlbumSummaryJob(IAlbumSummaryService albumSummaryService)
    {
        _albumSummaryService = albumSummaryService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using Activity? _ = Tracing.Source.StartActivity("Execut UpdateAllAlbumSummary job");

        await _albumSummaryService.UpdateAllAsync(context.CancellationToken);
    }
}
