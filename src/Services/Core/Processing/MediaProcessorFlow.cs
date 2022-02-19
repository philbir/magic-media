using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using OpenTelemetry.Trace;

namespace MagicMedia.Processing;

public class MediaProcessorFlow : IMediaProcessorFlow
{
    private readonly IMediaProcesserTaskFactory _taskFactory;

    public MediaProcessorFlow(
        IMediaProcesserTaskFactory taskFactory,
        IEnumerable<string> tasks)
    {
        _taskFactory = taskFactory;
        Tasks = tasks;
    }

    public IEnumerable<string> Tasks { get; }

    public async virtual Task ExecuteAsync(
        MediaProcessorContext context,
        CancellationToken cancellationToken)
    {
        using Activity? mainActivity = Tracing.Source.StartActivity($"Execute media processor flow");

        if (context.File is { })
        {
            mainActivity?.AddTag("file.id", context.File.Id);
        }

        if (context.Media is { })
        {
            mainActivity?.AddTag("media.id", context.Media.Id);
        }

        foreach (string taskName in Tasks)
        {
            IMediaProcessorTask instance = _taskFactory.GetTask(taskName);
            using Activity? taskActivity = Tracing.Source.StartActivity($"Execute task {taskName}");

            try
            {
                await instance.ExecuteAsync(context, cancellationToken);

                if (context.StopProcessing)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                taskActivity.RecordException(ex);
                throw;
            }
        }
    }
}
