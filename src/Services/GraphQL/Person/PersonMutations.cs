using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Mutation")]
    public partial class PersonMutations
    {
        private readonly IPersonService _personService;
        private readonly IGroupService _groupService;

        public PersonMutations(IPersonService personService, IGroupService groupService)
        { 
            _personService = personService;
            _groupService = groupService;
        }

        public async Task<UpdatePersonPayload> UpdatePersonAsync(
            UpdatePersonRequest input,
            CancellationToken cancellationToken)
        {
            Person person = await _personService.UpdatePersonAsync(input, cancellationToken);

            return new UpdatePersonPayload(person);
        }

        public async Task<CreateGroupPayload> CreateGroupAsync(
            string name,
            CancellationToken cancellationToken)
        {
            Group group = await _groupService.AddAsync(name, cancellationToken);

            return new CreateGroupPayload(group);
        }
    }
}
