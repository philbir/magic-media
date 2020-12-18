using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MongoDB.Bson;
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

        public async Task<SearchResult<Person>> SearchAsync(
            SearchPersonRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Person> filter = Builders<Person>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                filter &= Builders<Person>.Filter.Regex(
                    x => x.Name,
                    new BsonRegularExpression($".*{Regex.Escape(request.SearchText)}.*", "i"));
            }

            if (request.Groups is { } groups && groups.Any())
            {
                filter = filter & Builders<Person>.Filter.In(nameof(Person.Groups), groups);
            }

            if (request.AuthorizedOn is { } authorized && authorized.Any())
            {
                filter = filter & Builders<Person>.Filter.In(x => x.Id, authorized);
            }

            IFindFluent<Person, Person>? cursor = _mediaStoreContext.Persons.Find(filter);
            long totalCount = await cursor.CountDocumentsAsync(cancellationToken);

            List<Person> persons = await cursor
                .Skip(request.PageNr * request.PageSize)
                .Limit(request.PageSize)
                .ToListAsync();

            return new SearchResult<Person>(persons, (int)totalCount);
        }

        public async Task<Person> GetByIdAsync(
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
