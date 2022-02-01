using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace MagicMedia.Jobs;

public static class JobsServiceCollectionExtensions
{
    public static IMagicMediaServerBuilder AddJobs(this IMagicMediaServerBuilder builder)
    {
        builder.Services.AddSingleton<IJob, ImportNewMediaJob>();
        builder.Services.AddSingleton<IJob, UpdateAllAlbumSummaryJob>();
        builder.Services.AddSingleton<IJob, UpdateAllPersonSummaryJob>();
        builder.Services.AddSingleton<IJob, AzureComputerVisionAnalyseJob>();

        return builder;
    }
}
