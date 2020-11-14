﻿using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration
{
    internal class AlbumCollectionConfiguration :
        IMongoCollectionConfiguration<Album>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<Album> builder)
        {
            builder
                .WithCollectionName(CollectionNames.Album)
                .AddBsonClassMap<Album>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {

                });
        }
    }
}
