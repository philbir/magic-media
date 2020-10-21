using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using HotChocolate.DataLoader;
using MagicMedia.Store;

namespace MagicMedia.Api.GraphQL.DataLoaders
{
    public class PersonByIdDataLoader : BatchDataLoader<Guid, Person>
    {
        private readonly IPersonStore _personStore;

        public PersonByIdDataLoader(
            IBatchScheduler batchScheduler,
            IPersonStore personStore)
            : base(batchScheduler)
        {
            _personStore = personStore;
        }

        protected async override Task<IReadOnlyDictionary<Guid, Person>> LoadBatchAsync(
            IReadOnlyList<Guid> keys,
            CancellationToken cancellationToken)
        {
            IEnumerable<Person> persons = await _personStore.GetPersonsAsync(
                keys,
                cancellationToken);

            return persons.ToDictionary(x => x.Id);
        }
    }
}
