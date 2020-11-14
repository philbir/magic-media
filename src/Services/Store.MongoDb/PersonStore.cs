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

        public async Task<IEnumerable<Person>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Persons.AsQueryable()
                .ToListAsync(cancellationToken);
        }

        public async Task<Person> GetByIdAsnc(
            Guid id,
            CancellationToken cancellationToken)
        {
            Person person = await _mediaStoreContext.Persons.AsQueryable()
                .Where(x => x.Id == id)
                .SingleAsync(cancellationToken);

            return person;
        }

        public async Task<Person?> TryGetByNameAsync(
            string name,
            CancellationToken cancellationToken)
        {
            Person? person = await _mediaStoreContext.Persons.AsQueryable()
                .Where(x => x.Name.ToLower() == name.ToLower())
                .FirstOrDefaultAsync(cancellationToken);

            return person;
        }

        public async Task<Person> AddAsync(
            Person person,
            CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Persons.InsertOneAsync(
                person,
                options: null,
                cancellationToken);

            return person;
        }

        public async Task<Person> UpdateAsync(
            Person person,
            CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Persons.ReplaceOneAsync(
                x => x.Id == person.Id,
                person,
                options: new ReplaceOptions(),
                cancellationToken);

            return person;
        }

    }
}
