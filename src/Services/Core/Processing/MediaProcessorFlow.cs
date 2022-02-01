using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

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
        using Activity? mainActivity = Tracing.Core.StartActivity($"Execute Flow");

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
            //Log.Information("Execute Task {Name}", taskName);

            using Activity? taskActivity = Tracing.Core.StartActivity($"Execute Task {taskName}");

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
                //Log.Error(ex, "Error executing Task {Name}", taskName);
                throw;
            }
        }
    }
}
