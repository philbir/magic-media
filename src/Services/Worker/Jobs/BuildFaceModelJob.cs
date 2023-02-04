using System.Diagnostics;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.Telemetry;
using Quartz;

namespace MagicMedia.Jobs;

public class BuildFaceModelJob : IJob
{
    private readonly IFaceModelBuilderService _faceModelBuilder;

    public BuildFaceModelJob(IFaceModelBuilderService faceModelBuilder)
    {
        _faceModelBuilder = faceModelBuilder;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using Activity? activity = Tracing.Source.StartRootActivity(
            "Execute BuildFaceModel job");

        await _faceModelBuilder.BuildModelAsyc(context.CancellationToken);
    }
}
