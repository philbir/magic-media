using MagicMedia.Store;

namespace MagicMedia
{
    public static class MediaExtensions
    {
        public static MediaBlobData ToBlobDataRequest(this Media media)
        {
            return new MediaBlobData
            {
                Type = MediaBlobType.Media,
                Directory = media.Folder,
                Filename = media.Filename
            };
        }
    }
}
