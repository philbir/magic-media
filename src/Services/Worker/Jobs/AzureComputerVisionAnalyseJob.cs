using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MagicMedia.Store;
using MagicMedia.Telemetry;
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
        using Activity? activity = Tracing.Source.StartRootActivity(
            "Execute AzureComputerVisionAnalyse job");

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
