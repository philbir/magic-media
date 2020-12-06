using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class PersonResolvers
    {
        private readonly IPersonService _personService;
        private readonly IPersonTimelineService _personTimelineService;
        private readonly IGroupService _groupService;

        public PersonResolvers(
            IPersonService personService,
            IPersonTimelineService personTimelineService,
            IGroupService groupService)
        {
            _personService = personService;
            _personTimelineService = personTimelineService;
            _groupService = groupService;
        }

        public async Task<IEnumerable<Group>> GetGroupsAsync(
            Person person,
            CancellationToken cancellationToken)
        {
            if (person.Groups is { } groups && groups.Any())
            {
                return await _groupService.GetAsync(person.Groups, cancellationToken);
            }

            return new Group[0];
        }

        public async Task<MediaThumbnail?> GetThumbnailAsync(
            Person person,
            CancellationToken cancellationToken)
        {
            return await _personService.TryGetFaceThumbnailAsync(
                person.Id,
                cancellationToken);
        }

        public async Task<PersonTimeline> GetTimelineAsync(
            Person person,
            int itemsPerYear,
            CancellationToken cancellationToken)
        {
            return await _personTimelineService.BuildTimelineAsync(
                person.Id,
                itemsPerYear,
                cancellationToken);
        }
    }
}
