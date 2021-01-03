using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using MagicMedia.Thumbprint;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Playground
{
    public class ClientThumbprintLoader
    {
        private readonly MediaStoreContext _dbContext;
        private readonly IClientThumbprintService _thumbprintService;

        public ClientThumbprintLoader(
            MediaStoreContext dbContext,
            IClientThumbprintService thumbprintService)
        {
            _dbContext = dbContext;
            _thumbprintService = thumbprintService;
        }

        public async Task LoadAuditThumbprintsAsync()
        {
            List<AuditEvent> audits = await _dbContext.AuditEvents.AsQueryable()
                .Where(x => x.ThumbprintId == null)
                .ToListAsync();

            var todo = audits.Count();

            foreach (AuditEvent audit in audits)
            {
                Console.WriteLine($"{todo} - {audit.Client.IPAdddress}");
                string id = await _thumbprintService.GetOrCreateAsync(audit.Client, default);
                audit.ThumbprintId = id;

                audit.Client.IPAdddress = null;
                audit.Client.UserAgent = null;

                await _dbContext.AuditEvents.ReplaceOneAsync(
                    x => x.Id == audit.Id,
                    audit);

                todo--;
            }
        }
    }
}
