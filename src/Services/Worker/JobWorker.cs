using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia;
using MagicMedia.Jobs;
using MagicMedia.Video;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using Serilog;

namespace Worker;

public class JobWorker : BackgroundService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;
    private readonly IFFmpegInitializer _fFmpegInitializer;
    private readonly IEnumerable<JobScheduleOptions> _scheduleOptions;
    private readonly IEnumerable<IJob> _jobs;

    public JobWorker(
        ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        IFFmpegInitializer fFmpegInitializer,
        IEnumerable<JobScheduleOptions> scheduleOptions,
        IEnumerable<IJob> jobs)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
        _fFmpegInitializer = fFmpegInitializer;
        _scheduleOptions = scheduleOptions;
        _jobs = jobs;
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
        using Activity? activity = Tracing.Source.StartActivity("Start JobWorker");

        await _fFmpegInitializer.Intitialize();

        IScheduler scheduler = await _schedulerFactory
            .GetScheduler(cancellationToken);

        scheduler.JobFactory = _jobFactory;

        foreach (IJob job in _jobs)
        {
            Type jobType = job.GetType();
            activity?.AddEvent(new ActivityEvent($"Scheduling job {jobType.Name}"));

            JobScheduleOptions? options = _scheduleOptions
                .FirstOrDefault(x => x.Name == jobType.Name);

            if (options != null && options.Enabled)
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

        await scheduler.Start();
    }
}
