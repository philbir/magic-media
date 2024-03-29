using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;

namespace MagicMedia.Store;

public interface IPersonStore
{
    Task<Person> AddAsync(Person person, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Person>> GetAllAsync(CancellationToken cancellationToken);
    Task<Person> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Person>> GetPersonsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);
    Task<SearchResult<Person>> SearchAsync(SearchPersonRequest request, CancellationToken cancellationToken);
    Task<Person> TryGetByNameAsync(string name, CancellationToken cancellationToken);
    Task<Person> UpdateAsync(Person person, CancellationToken cancellationToken);
}
