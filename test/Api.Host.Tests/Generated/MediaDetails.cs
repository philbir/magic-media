namespace MagicMedia.Api.Host.Tests
{
    public class MediaDetails
        : IMediaDetails
    {
        public MediaDetails(
            IMedia mediaById)
        {
            MediaById = mediaById;
        }

        public IMedia MediaById { get; }
    }
}
