namespace MagicMedia
{
    public record MediaBlobData
    {
        public string Filename { get; init; }

        public string Directory { get; init; } = "/";

        public byte[] Data { get; init; }

        public MediaBlobType Type { get; set; } = MediaBlobType.Media;
    }
}
