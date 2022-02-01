using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Identity.Core.Services;

public interface ISmsService
{
    Task SendSmsAsync(string mobile, string message, CancellationToken cancellationToken);
}
