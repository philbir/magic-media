using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MagicMedia.Identity.Data;

namespace MagicMedia.Identity.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly IClientRepository _clientRepository;

        public ClientStore(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            MagicClient client = await _clientRepository.GetAsync(clientId, default);

            return client;
        }
    }
}
