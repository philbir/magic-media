namespace MagicMedia.Api.Host.Tests
{
    public class Camera
        : ICamera
    {
        public Camera(
            System.Guid id,
            string model,
            string make)
        {
            Id = id;
            Model = model;
            Make = make;
        }

        public System.Guid Id { get; }

        public string Model { get; }

        public string Make { get; }
    }
}
