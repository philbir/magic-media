using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Security
{
    public interface IUserContextFactory
    {
        Task<IUserContext> CreateAsync(CancellationToken cancellationToken);
        Task<IUserContext> CreateAsync(ClaimsPrincipal? principal, CancellationToken cancellationToken);
    }
}
