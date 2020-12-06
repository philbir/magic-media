using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface IPersonTimelineService
    {
        Task<PersonTimeline> BuildTimelineAsync(
            Guid personId,
            int itemsPerYear,
            CancellationToken cancellationToken);
    }

    public class PersonTimeline
    {
        public IEnumerable<PersonTimelineAge> Ages { get; set; }
    }

    public class PersonTimelineAge
    {
        public int Age { get; set; }

        public IEnumerable<MediaFace> Faces { get; set; }
    }
}
