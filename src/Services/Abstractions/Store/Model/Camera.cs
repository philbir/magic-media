using System;

namespace MagicMedia.Store;

public class Camera
{
    public Guid Id { get; set; }

    public string? Make { get; set; }

    public string? Model { get; set; }

    public string? Title { get; set; }
}
