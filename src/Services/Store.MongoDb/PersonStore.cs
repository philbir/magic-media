using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb
{
    public class PersonStore : IPersonStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public PersonStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task<IEnumerable<Person>> GetPersonsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Persons.AsQueryable()
                .Where(x => ids.ToList().Contains(x.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<Person> GetOrCreatePersonAsync(
            string name,
            CancellationToken cancellationToken)
        {
            Person person = await _mediaStoreContext.Persons.AsQueryable()
                .Where(x => x.Name.ToLower() == name.ToLower())
                .FirstOrDefaultAsync(cancellationToken);

            if (person == null)
            {
                person = new Person
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };

                await _mediaStoreContext.Persons.InsertOneAsync(
                    person,
                    options: null,
                    cancellationToken);
            }

            return person;
        }
    }
}
