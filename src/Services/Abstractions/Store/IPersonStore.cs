using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store
{
    public interface IPersonStore
    {
        Task<Person> GetOrCreatePersonAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<Person>> GetPersonsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    }
}
