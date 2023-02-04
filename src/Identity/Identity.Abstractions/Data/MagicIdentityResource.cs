using Duende.IdentityServer.Models;

namespace MagicMedia.Identity.Data;

public class MagicIdentityResource : IdentityResource
{
    public string Id => Name;
}
