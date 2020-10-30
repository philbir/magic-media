using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface IPersonService
    {
        Task<IEnumerable<Person>> GetAllAsync(CancellationToken cancellationToken);
        Task<Person> GetOrCreatePersonAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<Person>> GetPersonsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    }
}