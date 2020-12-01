using MagicMedia.Jobs;
using MagicMedia.Processing;
using MagicMedia.Video;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Worker
{
    public class JobWorker : BackgroundService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IFFmpegInitializer _fFmpegInitializer;

        public JobWorker(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IFFmpegInitializer fFmpegInitializer)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _fFmpegInitializer = fFmpegInitializer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            await _fFmpegInitializer.Intitialize();

            IScheduler scheduler = await _schedulerFactory
                .GetScheduler(cancellationToken);

            scheduler.JobFactory = _jobFactory;

            IJobDetail job = JobBuilder
                .Create<ImportNewMediaJob>()
                .WithIdentity("ImportMedia")
                .Build();

            ITrigger trigger = TriggerBuilder
                .Create()
                .WithIdentity("ImportMedia")
                .WithSimpleSchedule(c => c.WithIntervalInMinutes(30))
                .Build();

            await scheduler.ScheduleJob(job, trigger, cancellationToken);

            await scheduler.Start();
        }
    }
}
