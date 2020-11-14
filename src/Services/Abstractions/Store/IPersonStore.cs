using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store
{
    public interface IPersonStore
    {
        Task<Person> AddAsync(Person person, CancellationToken cancellationToken);
        Task<IEnumerable<Person>> GetAllAsync(CancellationToken cancellationToken);
        Task<Person> GetByIdAsnc(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<Person>> GetPersonsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken);

        Task<Person> TryGetByNameAsync(string name, CancellationToken cancellationToken);
        Task<Person> UpdateAsync(Person person, CancellationToken cancellationToken);
    }
}
