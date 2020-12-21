using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb
{
    public class UserStore : IUserStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public UserStore(MediaStoreContext mediaStoreContext)
        { 
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task<User> TryGetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Users.AsQueryable()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User> TryGetByPersonIdAsync(
            Guid personId,
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Users.AsQueryable()
                .Where(x => x.PersonId == personId)
                .FirstOrDefaultAsync(cancellationToken);
        }
        
        public async Task<IEnumerable<User>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Users.AsQueryable()
                .ToListAsync(cancellationToken);
        }

        public async Task<User> AddAsync(User user, CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Users.InsertOneAsync(
                user,
                DefaultMongoOptions.InsertOne,
                cancellationToken);

            return user;
        }

        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Users.ReplaceOneAsync(
                x => x.Id == user.Id,
                user,
                DefaultMongoOptions.Replace,
                cancellationToken);

            return user;
        }

        public async Task<SearchResult<User>> SearchAsync(SearchUserRequest request, CancellationToken cancellationToken)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                filter &= Builders<User>.Filter.Regex(
                    x => x.Name,
                    new BsonRegularExpression($".*{Regex.Escape(request.SearchText)}.*", "i"));
            }

            IFindFluent<User, User>? cursor = _mediaStoreContext.Users.Find(filter);
            long totalCount = await cursor.CountDocumentsAsync(cancellationToken);

            List<User> users = await cursor
                .Skip(request.PageNr * request.PageSize)
                .Limit(request.PageSize)
                .ToListAsync();

            return new SearchResult<User>(users, (int)totalCount);
        }
    }
}
