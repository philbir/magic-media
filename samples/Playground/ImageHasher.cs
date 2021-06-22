using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoenM.ImageHash.HashAlgorithms;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MagicMedia.Playground
{
    public class ImageHasher
    {
        private readonly MediaStoreContext _dbContext;
        private readonly IMediaService _mediaService;
        private readonly ISimilarMediaService _duplicateMediaService;

        public ImageHasher(MediaStoreContext dbContext, IMediaService mediaService, ISimilarMediaService duplicateMediaService)
        {
            _dbContext = dbContext;
            _mediaService = mediaService;
            _duplicateMediaService = duplicateMediaService;
        }

        public async Task GetDuplicatesAsync()
        {
            await _duplicateMediaService.GetDuplicatesAsync(default);

        }
        public async Task HashAsync()
        {
            List<Media> medias = await _dbContext.Medias.AsQueryable()
                .Where(x =>
                    x.State == MediaState.Active &&
                    x.MediaType == MediaType.Video &&
                    x.Hashes == null)
                .ToListAsync();

            var todo = medias.Count;

            var avgHasher = new AverageHash();
            var percHasher = new PerceptualHash();
            var diffHasher = new DifferenceHash();

            foreach (Media media in medias)
            {
                Console.WriteLine($"{todo} - {media.Id}");

                try
                {
                    var hashes = new List<MediaHash>();
                    //hashes.Add(new MediaHash
                    //{
                    //    Type = MediaHashType.FileHashSha256,
                    //    Value = media.OriginalHash
                    //});

                    //if (media.UniqueIdentifier != null)
                    //{
                    //    hashes.Add(new MediaHash
                    //    {
                    //        Type = MediaHashType.Identifiers,
                    //        Value = media.UniqueIdentifier
                    //    });
                    //}

                    if ( media.MediaType == MediaType.Image)
                    {
                        var fileName = _mediaService.GetFilename(media, MediaFileType.Original);

                        if (File.Exists(fileName))
                        {
                            using FileStream stream = File.OpenRead(fileName);
                            Image<Rgba32> image = await Image.LoadAsync<Rgba32>(stream);

                            hashes.Add(new MediaHash
                            {
                                Type = MediaHashType.ImageAverageHash,
                                Value = avgHasher.Hash(image).ToString()
                            });

                            hashes.Add(new MediaHash
                            {
                                Type = MediaHashType.ImagePerceptualHash,
                                Value = percHasher.Hash(image).ToString()
                            });

                            hashes.Add(new MediaHash
                            {
                                Type = MediaHashType.ImageDifferenceHash,
                                Value = diffHasher.Hash(image).ToString()
                            });
                        }
                        else
                        {
                            File.WriteAllText($@"C:\MagicMedia\Broken\{media.Id}.txt", $"NOT_FOUND:{fileName}");
                        }
                    }

                    UpdateDefinition<Media> update = Builders<Media>.Update.Set(x => x.Hashes, hashes);
                    await _dbContext.Medias.UpdateOneAsync(x => x.Id == media.Id, update);

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
