namespace MagicMedia.Processing
{
    public static class MediaProcessorTaskNames
    {
        public static readonly string GenerateThumbnails = "GenerateThumbnail";
        public static readonly string GenerateWebImage = "GenerateWebImage";
        public static readonly string ExtractMetadata = "ExtractMetadata";
        public static readonly string BuildFaceData = "BuildFaceData";
        public static readonly string PredictPersons = "PredictPersons";
        public static readonly string AutoOrient = "AutoOrient";
        public static readonly string SaveMedia = "SaveMedia";
        public static readonly string CleanUpSource = "CleanUpSource";
        public static readonly string SaveFaces = "SaveFaces";
        public static readonly string ParseDateTaken = "ParseDateTaken";

        public static readonly string ExtractVideoData = "ExtractVideoData";
        public static readonly string BuildGifVideoPreview = "BuildGifVideoPreview";
        public static readonly string BuildVideoPreview = "BuildVideoPreview";
    }
}
