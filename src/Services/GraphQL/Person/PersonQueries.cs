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
}
