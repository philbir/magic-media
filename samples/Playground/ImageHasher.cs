using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Playground
{
    public class ImageHasher
    {
        private readonly MediaStoreContext _dbContext;
        private readonly IMediaService _mediaService;

        public ImageHasher(MediaStoreContext dbContext, IMediaService mediaService)
        {
            _dbContext = dbContext;
            _mediaService = mediaService;
        }

        public async Task HashAsync()
        {
            List<Media> medias = await _dbContext.Medias.AsQueryable()
                .Where(x =>
                    x.State == MediaState.Active &&
                    x.MediaType == MediaType.Image &&
                    x.ImageHash == null)
                .ToListAsync();

            var todo = medias.Count;

            var hasher = new AverageHash();

            foreach (Media media in medias)
            {
                Console.WriteLine($"{todo} - {media.Id}");

                try
                {

                    var fileName = _mediaService.GetFilename(media, MediaFileType.Original);

                    if (File.Exists(fileName))
                    {
                        var hash = hasher.Hash(File.OpenRead(fileName));

                        UpdateDefinition<Media> update = Builders<Media>.Update.Set(x => x.ImageHash, hash.ToString());
                        await _dbContext.Medias.UpdateOneAsync(x => x.Id == media.Id, update);
                    }
                    else
                    {
                        File.WriteAllText($@"C:\MagicMedia\Broken\{media.Id}.txt", $"NOT_FOUND:{fileName}");
                    }
                }
                catch (Exception ex)
                {
                    File.WriteAllText($@"C:\MagicMedia\Broken\{media.Id}.txt", $"ERROR:{ex.Message}");
                }

                todo--;
            }
        }
    }
}
