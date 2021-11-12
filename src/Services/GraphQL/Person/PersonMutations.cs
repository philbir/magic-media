using HotChocolate.AspNetCore.Authorization;
using MagicMedia.Authorization;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [Authorize(Apply = ApplyPolicy.BeforeResolver, Policy = AuthorizationPolicies.Names.PersonEdit)]
    [ExtendObjectType(RootTypes.Mutation)]
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

        [Authorize(Apply = ApplyPolicy.BeforeResolver, Policy = AuthorizationPolicies.Names.PersonDelete)]
        [GraphQLName("Person_Delete")]
        public async Task<DeleteGroupPayload> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _personService.DeleteAsync(id, cancellationToken);

            return new DeleteGroupPayload(id);
        }
    }
}
