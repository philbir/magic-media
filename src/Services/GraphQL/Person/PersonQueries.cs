using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class PersonQueries
    {
        private readonly IPersonService _personService;

        public PersonQueries(IPersonService personService)
        {
            _personService = personService;
        }

        public async Task<IEnumerable<Person>> GetPersonsAsync(CancellationToken cancellationToken)
        {
            return await _personService.GetAllAsync(cancellationToken);
        }
    }

    [ExtendObjectType(Name = "Mutation")]
    public class PersonMutations
    {
        private readonly IPersonService _personService;

        public PersonMutations(IPersonService personService)
        { 
            _personService = personService;
        }

        public async Task<UpdatePersonPayload> UpdatePersonAsync(
            UpdatePersonRequest input,
            CancellationToken cancellationToken)
        {
            Person person = await _personService.UpdatePersonAsync(input, cancellationToken);

            return new UpdatePersonPayload(person);
        }
    }


    public class UpdatePersonPayload : Payload
    {
        public Person? Person { get; }

        public UpdatePersonPayload(Person person)
        {
            Person = person;
        }

        public UpdatePersonPayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }
}
