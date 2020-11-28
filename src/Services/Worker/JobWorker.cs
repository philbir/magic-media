using MagicMedia.Jobs;
using MagicMedia.Processing;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace Worker
{
    public class JobWorker : BackgroundService
    {
        private readonly IMediaSourceScanner _mediaSourceScanner;
        private readonly IBusControl _busControl;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;

        public JobWorker(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
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
            FFmpeg.SetExecutablesPath(Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg"));


            //await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);

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
