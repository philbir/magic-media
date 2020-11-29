using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using MagicMedia.Identity.Data;

namespace MagicMedia.Identity.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IClientRepository _clientRepository;

        public CorsPolicyService(
            IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            HashSet<string> allowedOrigins = await GetAllClientOrigins();
            bool allowed = allowedOrigins.Contains(origin);

            return allowed;
        }

        private async Task<HashSet<string>> GetAllClientOrigins()
        {
            HashSet<string>? origins = await _clientRepository
                .GetAllClientOrigins();

            HashSet<string> redirectUOrigins =
                    await _clientRepository.GetAllClientRedirectUriAsync();

            return RemovePathFromUrl(origins.Concat(redirectUOrigins));
        }

        private HashSet<string> RemovePathFromUrl(IEnumerable<string> urls)
        {
            var urlsWithoutPath = new HashSet<string>();

            foreach (var url in urls)
            {
                if (url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    var uri = new Uri(url);
                    var port = !uri.IsDefaultPort ?
                         ":" + uri.Port.ToString(CultureInfo.InvariantCulture) : "";

                    urlsWithoutPath.Add(
                        $"{uri.Scheme}://{uri.Host}{port}");
                }
            }

            return urlsWithoutPath;
        }
    }
}
