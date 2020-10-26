using System;

namespace MagicMedia.Identity.Services
{
    public interface ITotpCodeService
    {
        string Generate(string secret);
        bool Validate(string code, string secret);
    }
}
