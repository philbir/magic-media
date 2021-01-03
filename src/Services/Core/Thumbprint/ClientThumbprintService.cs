using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Store;
using Microsoft.Extensions.Caching.Memory;

namespace MagicMedia.Thumbprint
{
    public class ClientThumbprintService : IClientThumbprintService
    {
        private readonly IClientThumbprintStore _thumbprintStore;
        private readonly IUserAgentInfoService _userAgentInfoService;
        private readonly IGeoIPLocationService _geoIPLocationService;
        private readonly IMemoryCache _memoryCache;

        public ClientThumbprintService(
            IClientThumbprintStore thumbprintStore,
            IUserAgentInfoService userAgentInfoService,
            IGeoIPLocationService geoIPLocationService,
            IMemoryCache memoryCache)
        {
            _thumbprintStore = thumbprintStore;
            _userAgentInfoService = userAgentInfoService;
            _geoIPLocationService = geoIPLocationService;
            _memoryCache = memoryCache;
        }

        public async Task<string> GetOrCreateAsync(
            ClientInfo clientInfo,
            CancellationToken cancellationToken)
        {
            var id = ComputeHash(clientInfo);

            ClientThumbprint? thumbprint = await _memoryCache.GetOrCreateAsync(id, async (entry) =>
            {
               entry.SlidingExpiration = TimeSpan.FromHours(6);
               return await _thumbprintStore
                   .TryGetByIdAsync(id, cancellationToken);
            });

            if (thumbprint != null)
            {
                return thumbprint.Id;
            }

            thumbprint = new ClientThumbprint
            {
                Id = id,
                CreatedAt = DateTime.UtcNow
            };

            thumbprint.UserAgent = _userAgentInfoService.Parse(clientInfo.UserAgent);
            thumbprint.IpInfo = await _geoIPLocationService.LookupAsync(
                clientInfo.IPAdddress,
                cancellationToken);

            await _thumbprintStore.AddAsync(thumbprint, cancellationToken);
            _memoryCache.Set(id, thumbprint);

            return id;
        }

        public async Task<IEnumerable<ClientThumbprint>> GetManyAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken)
        {
            return await _thumbprintStore.GetManyAsync(ids, cancellationToken);
        }

        private string ComputeHash(ClientInfo clientInfo)
        {
            return $"{clientInfo.IPAdddress}_{clientInfo.UserAgent}".ComputeSha256();
        }
    }
}
