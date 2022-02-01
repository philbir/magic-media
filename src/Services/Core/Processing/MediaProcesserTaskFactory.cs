using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicMedia.Processing;

public class MediaProcesserTaskFactory : IMediaProcesserTaskFactory
{
    private readonly IEnumerable<IMediaProcessorTask> _tasks;

    public MediaProcesserTaskFactory(IEnumerable<IMediaProcessorTask> tasks)
    {
        _tasks = tasks;
    }

    public IMediaProcessorTask GetTask(string name)
    {
        IMediaProcessorTask? task = _tasks.FirstOrDefault(x => x.Name == name);

        if (task is null)
        {
            throw new InvalidOperationException($"No Task with name '{name}' registred");
        }

        return task;
    }
}
