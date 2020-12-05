using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface IPersonService
    {
        Task<IEnumerable<Person>> GetAllAsync(CancellationToken cancellationToken);
        Task<Person> GetOrCreatePersonAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<Person>> GetPersonsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
        Task<SearchResult<Person>> SearchAsync(SearchPersonRequest request, CancellationToken cancellationToken);
        Task<MediaThumbnail?> TryGetFaceThumbnailAsync(Guid personId, CancellationToken cancellationToken);
        Task UpdateAllSummaryAsync(CancellationToken cancellationToken);
        Task<Person> UpdatePersonAsync(UpdatePersonRequest request, CancellationToken cancellationToken);
        Task<Person> UpdateSummaryAsync(Person person, CancellationToken cancellationToken);
    }
}
