using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Extensions.Context;
using Squadron;
using Xunit;

namespace MagicMedia.Store.MongoDb.Tests
{
    public class PersonStoreTests : IClassFixture<MongoResource>
    {
        private readonly MongoResource _mongo;

        public PersonStoreTests(MongoResource mongo)
        {
            _mongo = mongo;
        }

        [Fact]
        public async Task GetOrCreatePerson_New_PersonCreated()
        {
            // Arrange
            IMongoDatabase db = _mongo.CreateDatabase();

            var dbContext = new MediaStoreContext(new MongoOptions
            {
                ConnectionString = _mongo.ConnectionString,
                DatabaseName = db.DatabaseNamespace.DatabaseName
            });

            var personStore = new PersonStore(dbContext);
            var newPerson = new Person
            {
                Id = Guid.NewGuid(),
                Name = "Bart",
                DateOfBirth = new DateTime(1980, 4, 2),
                Groups = new List<Guid>() { Guid.NewGuid() }
            };

            // Act
            Person person = await personStore
                .AddAsync(newPerson, default);

            // Assert
            Person cratedPerson = await dbContext.Persons.AsQueryable()
                .Where(x => x.Id == newPerson.Id)
                .FirstOrDefaultAsync();

            cratedPerson.Name.Should().Be(person.Name);
        }
    }
}
