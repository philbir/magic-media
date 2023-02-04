namespace MagicMedia.Api.Host.Tests
{
    public interface ICamera
    {
        System.Guid Id { get; }

        string Model { get; }

        string Make { get; }
    }
}
