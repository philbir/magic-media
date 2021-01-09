using MagicMedia.Identity.Data;

namespace MagicMedia.Identity.Services
{
    public interface IDemoUserService
    {
        bool IsDemoMode { get; }

        User? GetDemoUser();
    }
}