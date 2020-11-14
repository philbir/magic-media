using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface IAgeOperationsService
    {
        int? CalculateAge(DateTimeOffset? dateTaken, DateTime dateOfBirth);
        Task UpdateAgesByPersonAsync(Person person, CancellationToken cancellationToken);
        Task UpdateAgesByPersonAsync(Guid personId, CancellationToken cancellationToken);
    }
}