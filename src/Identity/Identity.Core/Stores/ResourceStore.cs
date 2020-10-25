using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MagicMedia.Identity.Data;

namespace MagicMedia.Identity.Stores
{
    public class ResourceStore : IResourceStore
    {
        private readonly IApiResourceRepository _apiResourceRepository;
        private readonly IApiScopeRepository _apiScopeRepository;
        private readonly IIdentityResourceRepository _identityResourceRepository;

        public ResourceStore(
            IApiResourceRepository apiResourceRepository,
            IApiScopeRepository apiScopeRepository,
            IIdentityResourceRepository identityResourceRepository)
        {
            _apiResourceRepository = apiResourceRepository;
            _apiScopeRepository = apiScopeRepository;
            _identityResourceRepository = identityResourceRepository;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(
            IEnumerable<string> apiResourceNames)
        {
            return await _apiResourceRepository.GetByNameAsync(apiResourceNames, default);
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(
            IEnumerable<string> scopeNames)
        {
            return await _apiResourceRepository.GetByScopeNameAsync(scopeNames, default);
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(
            IEnumerable<string> scopeNames)
        {
            return await _apiScopeRepository.GetByNameAsync(scopeNames, default);
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(
            IEnumerable<string> scopeNames)
        {
            return await _identityResourceRepository.GetByNameAsync(scopeNames, default);
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            Task<IEnumerable<MagicIdentityResource>>? idResTask = _identityResourceRepository
                .GetAllAsync(default);

            Task<IEnumerable<ApiScope>>? apiScopeTask = _apiScopeRepository.GetAllAsync(default);

            Task<IEnumerable<MagicApiResource>>? apiResTask = _apiResourceRepository
                .GetAllAsync(default);

            await Task.WhenAll(idResTask, apiResTask, apiScopeTask);

            return new Resources(idResTask.Result, apiResTask.Result, apiScopeTask.Result);
        }
    }
}
