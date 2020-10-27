using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Store.MongoDb;
using MongoDB.Driver.GridFS;
using Model = MagicMedia.Store;

namespace MagicMedia.Api.Host.Tests.Infrastructure
{
    public class DataSeeder
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public DataSeeder(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task SeedIntialDataAsync()
        {
            Model.Media media = DefaultMedia;
            IEnumerable<Model.MediaFace> faces = GetDefaultFaces(media.Id).ToList();

            await _mediaStoreContext.Medias.InsertOneAsync(media);

            await _mediaStoreContext.Cameras.InsertOneAsync(DefaultCamera);

            await _mediaStoreContext.Persons.InsertOneAsync(DefaultPerson);

            await _mediaStoreContext.Faces.InsertManyAsync(
                GetDefaultFaces(DefaultMedia.Id));

            IGridFSBucket gridFs = _mediaStoreContext.CreateGridFsBucket();

            foreach (Model.MediaThumbnail thumb in media.Thumbnails)
            {
                await gridFs.UploadFromBytesAsync(thumb.Id.ToString("N"), thumb.Data);
            }

            foreach (Model.MediaFace face in faces)
            {
                await gridFs.UploadFromBytesAsync(
                    face.Thumbnail.Id.ToString("N"),
                    face.Thumbnail.Data);
            }
        }

        public static Model.Media DefaultMedia => new Model.Media
        {
            Id = Guid.Parse("909fc3d7-8ea6-46f8-90f6-897c80bedaab"),
            Filename = "Test01.jpg",
            DateTaken = new DateTime(2020, 1, 1),
            CameraId = DefaultCamera.Id,
            GeoLocation = new GeoLocation
            {
                Type = "gps",
                GeoHash = "abc",
                Address = new GeoAddress
                {
                    City = "Zürich"
                }
            },
            Dimension = new MagicMedia.MediaDimension
            {
                Height = 600,
                Width = 800
            },
            Thumbnails = GetDefaultThumbnails().ToList()
        };

        public static IEnumerable<Model.MediaFace> GetDefaultFaces(Guid mediaId)
        {
            yield return new Model.MediaFace
            {
                MediaId = mediaId,
                Id = Guid.NewGuid(),
                Box = new MagicMedia.ImageBox
                {
                    Left = 10,
                    Top = 10,
                    Bottom = 10,
                    Right = 10
                },
                Encoding = new List<double> { 1, 2, 3 },
                State = Model.FaceState.New,
                RecognitionType = Model.FaceRecognitionType.Computer,
                Thumbnail = new Model.MediaThumbnail
                {
                    Id = Guid.NewGuid(),
                    Data = new byte[1],
                    Dimensions = new MagicMedia.MediaDimension
                    {
                        Height = 100,
                        Width = 100
                    },
                    Format = "jpg",
                    Size = MagicMedia.ThumbnailSizeName.S
                },
                PersonId = DefaultPerson.Id
            };
        }

        public static IEnumerable<Model.MediaThumbnail> GetDefaultThumbnails()
        {
            yield return new Model.MediaThumbnail
            {
                Id = Guid.Parse("611ac7e6-33fe-4168-b9ee-c8295c537e6d"),
                Data = new byte[1],
                Dimensions = new MagicMedia.MediaDimension
                {
                    Height = 100,
                    Width = 100
                },
                Format = "jpg",
                Size = MagicMedia.ThumbnailSizeName.S
            };

            yield return new Model.MediaThumbnail
            {
                Id = Guid.Parse("ed4bdf98-2d38-48f7-ac29-e0dadbd203e7"),
                Data = new byte[1],
                Format = "webp",
                Dimensions = new MagicMedia.MediaDimension
                {
                    Height = 200,
                    Width = 200
                },
                Size = MagicMedia.ThumbnailSizeName.M
            };
        }

        public static Model.Person DefaultPerson => new Model.Person
        {
            Id = Guid.Parse("39f7ca98-bc7d-4097-a02e-cd11a92fbbee"),
            Name = "Bart",
            DateOfBirth = new DateTime(1980, 5, 2),
        };

        public static Model.Camera DefaultCamera => new Model.Camera
        {
            Id = Guid.Parse("28d9b4ff-a749-495d-b5cb-b31c90cf0c77"),
            Make = "Foo",
            Model = "Bar"
        };
    }
}
