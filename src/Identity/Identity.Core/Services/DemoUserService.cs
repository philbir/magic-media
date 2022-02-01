using System;
using MagicMedia.Identity.Data;
using Microsoft.Extensions.Configuration;

namespace MagicMedia.Identity.Services;

public class DemoUserService : IDemoUserService
{
    public DemoUserService(bool isDemoMode, IConfiguration configuration)
    {
        DemoUserOptions? options = configuration
            .GetSection("Identity:DemoUser")
            .Get<DemoUserOptions>();

        if (isDemoMode && options != null)
        {
            IsDemoMode = isDemoMode;
            _demoUser = new User
            {
                Id = options.Id,
                Name = options.Name
            };
        }
    }

    public bool IsDemoMode { get; }

    private User? _demoUser;

    public User? GetDemoUser()
    {
        return _demoUser;
    }
}

public class DemoUserOptions
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
}
