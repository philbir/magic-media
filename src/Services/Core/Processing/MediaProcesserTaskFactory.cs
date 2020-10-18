using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicMedia.Processing
{
    public class MediaProcesserTaskFactory : IMediaProcesserTaskFactory
    {
        private readonly IEnumerable<IMediaProcesserTask> _tasks;

        public MediaProcesserTaskFactory(IEnumerable<IMediaProcesserTask> tasks)
        {
            _tasks = tasks;
        }

        public IMediaProcesserTask GetTask(string name)
        {
            IMediaProcesserTask? task = _tasks.FirstOrDefault(x => x.Name == name);

            if ( task is null)
            {
                throw new InvalidOperationException($"No Task with name '{name}' registred");
            }

            return task;
        }
    }
}
