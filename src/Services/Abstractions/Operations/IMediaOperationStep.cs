using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Operations
{
    public interface IMediaOperationStep
    {
        Task<MediaOperationStepResult> ExecuteAsync(MediaOperationStepContext context);

        string Name { get; }
    }

    public class MediaOperationStepResult
    {
        public MediaOperationStepState State { get; set; }

        public IList<string> Messages { get; set; } = new List<string>();
    }
}
