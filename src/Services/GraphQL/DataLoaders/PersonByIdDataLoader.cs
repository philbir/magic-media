using GreenDonut;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.DataLoaders
{
    public class GroupsByPersonIdDataLoader : BatchDataLoader<Guid, Group>
    {
        private readonly IGroupService _groupService;

        public GroupsByPersonIdDataLoader(
            IBatchScheduler batchScheduler,
            IGroupService groupService)
            : base(batchScheduler)
        {
            _groupService = groupService;
        }

        protected override Task<IReadOnlyDictionary<Guid, Group>> LoadBatchAsync(
            IReadOnlyList<Guid> keys,
            CancellationToken cancellationToken)
        {
            return null;
        }
    }

    public class PersonByIdDataLoader : BatchDataLoader<Guid, Person>
    {
        private readonly IPersonService _personService;

        public PersonByIdDataLoader(
            IBatchScheduler batchScheduler,
            IPersonService personService)
            : base(batchScheduler)
        {
            _personService = personService;
        }

        protected async override Task<IReadOnlyDictionary<Guid, Person>> LoadBatchAsync(
            IReadOnlyList<Guid> keys,
            CancellationToken cancellationToken)
        {
            IEnumerable<Person> persons = await _personService.GetPersonsAsync(
                keys,
                cancellationToken);

            return persons.ToDictionary(x => x.Id);
        }
    }
}
