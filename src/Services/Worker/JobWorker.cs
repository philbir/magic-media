using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia;
using MagicMedia.Jobs;
using MagicMedia.Telemetry;
using MagicMedia.Video;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace Worker;

public class JobWorker(
    ISchedulerFactory schedulerFactory,
    IJobFactory jobFactory,
    IFFmpegInitializer fFmpegInitializer,
    IEnumerable<JobScheduleOptions> scheduleOptions,
    IEnumerable<IJob> jobs)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        using Activity? activity = Tracing.Source.StartRootActivity("Start JobWorker");

        await fFmpegInitializer.Intitialize();

        IScheduler scheduler = await schedulerFactory
            .GetScheduler(cancellationToken);

        scheduler.JobFactory = jobFactory;

        foreach (IJob job in jobs)
        {
            Type jobType = job.GetType();
            activity?.AddEvent(new ActivityEvent($"Scheduling job {jobType.Name}"));

            App.Log.SchedulingJob(jobType.Name);

            JobScheduleOptions? options = scheduleOptions
                .FirstOrDefault(x => x.Name == jobType.Name);

            if (options is { Enabled: true })
            {
                IJobDetail jobDetail = JobBuilder
                    .Create(jobType)
                    .WithIdentity(jobType.Name)
                    .Build();

                TriggerBuilder triggerBuilder = TriggerBuilder
                    .Create()
                    .WithIdentity(jobType.Name);

                if (options.Interval.HasValue)
                {
                    triggerBuilder.WithSimpleSchedule(s => s
                        .WithInterval(options.Interval.Value)
                        .RepeatForever());

                    activity?.AddEvent(new ActivityEvent(
                        $"Schedule job {jobType.Name} with intervall: {options.Interval}"));
                }
                else
                {
                    triggerBuilder.WithCronSchedule(options.Cron!);

                    activity?.AddEvent(new ActivityEvent(
                        $"Schedule job {jobType.Name} with cron expression: {options.Cron}"));
                }

                await scheduler.ScheduleJob(jobDetail, triggerBuilder.Build(), cancellationToken);
            }
            else
            {
                activity?.AddEvent(new ActivityEvent(
                    $"Job {jobType.Name} is not enabled"));
            }
        }

        await scheduler.Start(cancellationToken);
    }
}

public static partial class LoggerExtensions
{
    [LoggerMessage(1, LogLevel.Information, "Scheduling job {jobName}", EventName = "SchedulingJob")]
    public static partial void SchedulingJob(this ILogger logger, string jobName);
}
