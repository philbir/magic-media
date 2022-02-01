using MagicMedia.Store;

namespace MagicMedia.GraphQL;

public class AddItemToAlbumPayload : Payload
{
    public AddItemToAlbumPayload(Album album)
    {
        Album = album;
    }

    public Album Album { get; }
}
