using System.Threading.Tasks;
using Quartz;

namespace MagicMedia.Jobs;

public class UpdateAllPersonSummaryJob : IJob
{
    private readonly IPersonService _personService;

    public UpdateAllPersonSummaryJob(IPersonService personService)
    {
        _personService = personService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _personService.UpdateAllSummaryAsync(context.CancellationToken);
    }
}
