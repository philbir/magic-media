using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class PersonResolvers
    {
        private readonly IPersonService _personService;
        private readonly IPersonTimelineService _personTimelineService;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;

        public PersonResolvers(
            IPersonService personService,
            IPersonTimelineService personTimelineService,
            IGroupService groupService,
            IUserService userService)
        {
            _personService = personService;
            _personTimelineService = personTimelineService;
            _groupService = groupService;
            _userService = userService;
        }

        public async Task<User?> GetUserAsync(Person person, CancellationToken cancellationToken)
        {
            return await _userService.TryGetByPersonIdAsync(person.Id, cancellationToken);
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
