using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class PersonQueries
    {
        private readonly IPersonService _personService;
        private readonly IGroupService _groupService;

        public PersonQueries(IPersonService personService, IGroupService groupService)
        {
            _personService = personService;
            _groupService = groupService;
        }

        public async Task<IEnumerable<Person>> GetPersonsAsync(CancellationToken cancellationToken)
        {
            return await _personService.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<Group>> GetGroupsAsync(CancellationToken cancellationToken)
        {
            return await _groupService.GetAllAsync(cancellationToken);
        }
    }
}
