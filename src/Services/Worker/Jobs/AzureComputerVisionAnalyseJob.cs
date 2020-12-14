using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Store;
using Quartz;
using Serilog;

namespace MagicMedia.Jobs
{
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
            Log.Information("Executing AzureComputerVisionAnalyse job");

            await _cloudAIMediaProcessing.ProcessNewBySourceAsync(
                AISource.AzureCV,
                context.CancellationToken);
        }
    }
}
