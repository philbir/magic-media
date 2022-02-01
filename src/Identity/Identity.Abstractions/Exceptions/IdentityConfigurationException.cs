using System;

namespace MagicMedia.Identity.Exceptions;

public class IdentityConfigurationException : Exception
{
    public IdentityConfigurationException(string? message) : base(message)
    {
    }
}
