using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia
{
    public class PersonService : IPersonService
    {
        private readonly IPersonStore _personStore;
        private readonly IBus _bus;

        public PersonService(IPersonStore personStore, IBus bus)
        {
            _personStore = personStore;
            _bus = bus;
        }

        public async Task<IEnumerable<Person>> GetPersonsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            return await _personStore.GetPersonsAsync(ids, cancellationToken);
        }

        public async Task<Person> GetOrCreatePersonAsync(
            string name,
            CancellationToken cancellationToken)
        {
            Person? person = await _personStore.TryGetByNameAsync(name, cancellationToken);

            if (person == null)
            {
                person = new Person
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };
                person = await _personStore.AddAsync(person, cancellationToken);

                await _bus.Publish(new PersonUpdatedMessage(person.Id, "New"));
            }

            return person;
        }
    }
}
