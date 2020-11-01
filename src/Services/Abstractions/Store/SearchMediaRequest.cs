namespace MagicMedia.Store
{
    public class SearchMediaRequest
    {
        public int? PageSize { get; set; } = 100;

        public ThumbnailSizeName ThumbnailSize { get; set; }
    }
}
