namespace MagicMedia.Store
{
    public class SearchMediaRequest
    {
        public int PageSize { get; set; }

        public ThumbnailSizeName ThumbnailSize { get; set; }
    }
}
