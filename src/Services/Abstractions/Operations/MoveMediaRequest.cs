using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Operations
{
    public class MoveMediaRequest
    {
        public IEnumerable<Guid> Ids { get; set; }

        public string NewLocation { get; set; }
    }

    public class MediaOperationResult
    {
        public Guid Id { get; set; }
    }

    public class MediaOperation
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public IEnumerable<MediaOperationTask> Tasks { get; set; }
    }

    public class MediaOperationTask
    {
        public OperationEntityIdentifier Entity { get; set; }

        public IDictionary<string, object> Data { get; set; }

        public string Name { get; set; }

        public IEnumerable<MediaOperationStep> Steps { get; set; }
    }

    public class MediaOperationStep
    {
        public string Name { get; set; }

        public MediaOperationStepState State { get; set; }

        public IList<string> Messages { get; set; }
    }

    public enum MediaOperationStepState
    {
        New,
        InProgress,
        Success,
        Failed
    }

    public class OperationEntityIdentifier
    {
        public Guid Id { get; init; }

        public string Type { get; init; }
    }
}
