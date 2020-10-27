using System;
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
            string personName = "Bart";

            // Act
            Person person = await personStore
                .GetOrCreatePersonAsync(personName, default);

            // Assert
            Person cratedPerson = await dbContext.Persons.AsQueryable()
                .Where(x => x.Name == personName)
                .FirstOrDefaultAsync();

            cratedPerson.Name.Should().Be(personName);
        }

        [Fact]
        public async Task GetOrCreatePerson_ExistingPerson_ExistingReturned()
        {
            // Arrange
            IMongoDatabase db = _mongo.CreateDatabase();

            var dbContext = new MediaStoreContext(new MongoOptions
            {
                ConnectionString = _mongo.ConnectionString,
                DatabaseName = db.DatabaseNamespace.DatabaseName
            });

            var existingPerson = new Person
            {
                Id = Guid.NewGuid(),
                Name = "Bart"
            };
            await dbContext.Persons.InsertOneAsync(
                existingPerson);

            PersonStore personStore = new PersonStore(dbContext);

            // Act
            Person person = await personStore
                .GetOrCreatePersonAsync(existingPerson.Name, default);

            // Assert
            person.Id.Should().Be(existingPerson.Id);
        }
    }
}
