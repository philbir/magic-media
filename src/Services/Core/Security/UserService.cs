using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;

namespace MagicMedia.Security
{
    public class UserService : IUserService
    {
        private readonly IUserStore _userStore;
        private readonly IPersonService _personService;

        public UserService(IUserStore userStore, IPersonService personService)
        {
            _userStore = userStore;
            _personService = personService;
        }

        public async Task<User> TryGetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _userStore.TryGetByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _userStore.GetAllAsync(cancellationToken);
        }

        public async Task<User> TryGetByPersonIdAsync(
            Guid personId,
            CancellationToken cancellationToken)
        {
            return await _userStore.TryGetByPersonIdAsync(personId, cancellationToken);
        }

        public async Task<User> CreateFromPersonAsync(CreateUserFromPersonRequest request, CancellationToken cancellationToken)
        {
            //TODO: Validate if there is allready a user for this person

            Person person = await _personService.GetByIdAsync(request.PersonId, cancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = person.Name,
                Email = request.Email,
                PersonId = person.Id
            };


            await _userStore.AddAsync(user, cancellationToken);

            return user;
        }


    }
}
