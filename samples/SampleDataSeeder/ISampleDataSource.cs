using System.Threading;
using System.Threading.Tasks;

namespace SampleDataSeeder
{
    public interface ISampleDataSource
    {
        Task<SampleMedia> LoadAsync(CancellationToken cancellationToken);
    }
}
