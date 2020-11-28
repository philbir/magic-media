using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Processing
{
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
            foreach (string taskName in Tasks)
            {
                IMediaProcessorTask instance = _taskFactory.GetTask(taskName);
                await instance.ExecuteAsync(context, cancellationToken);
            }
        }
    }
}
