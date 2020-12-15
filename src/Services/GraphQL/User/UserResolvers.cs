using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class UserResolvers
    {
        private readonly IPersonService _personService;

        public UserResolvers(IPersonService personService)
        {
            _personService = personService;
        }

        public async Task<Person?> GetPersonAsync(User user, CancellationToken cancellationToken)
        {
            if (user.PersonId.HasValue)
            {
                return await _personService.GetByIdAsync(user.PersonId.Value, cancellationToken);
            }

            return null;
        }
    }
}
