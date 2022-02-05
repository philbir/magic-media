using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MagicMedia.Store;
using OpenTelemetry.Trace;
using Quartz;
using Serilog;

namespace MagicMedia.Jobs;

public class AzureComputerVisionAnalyseJob : IJob
{
    private readonly IMediaAIService _cloudAIMediaProcessing;

    public AzureComputerVisionAnalyseJob(
        IMediaAIService cloudAIMediaProcessing)
    {
        _cloudAIMediaProcessing = cloudAIMediaProcessing;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using Activity? activity = Tracing.Source.CreateActivity(
            "Execute AzureComputerVisionAnalyse job",
            ActivityKind.Internal);

        try
        {

            await _cloudAIMediaProcessing.ProcessNewBySourceAsync(
                AISource.AzureCV,
                context.CancellationToken);
        }
        catch (Exception ex)
        {
            activity.RecordException(ex);
        }

    }
}
