namespace MagicMedia.Store
{
    public class MediaAIObject
    {
        public AISource Source { get; set; }

        public string Name { get; set; }

        public double Confidence { get; set; }

        public ImageBox Box { get; set; }
    }
}
