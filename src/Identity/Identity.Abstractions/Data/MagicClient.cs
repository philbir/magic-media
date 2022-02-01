using System.Collections.Generic;
using Duende.IdentityServer.Models;

namespace MagicMedia.Identity.Data;

public class MagicClient : Client
{
    public string Id => ClientId;

    public IEnumerable<EnabledProvider>? EnabledProviders { get; set; }
}

public class EnabledProvider
{
    public string? Name { get; set; }

    public bool RequestMfa { get; set; }
}
