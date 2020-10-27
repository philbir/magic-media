using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace MagicMedia.Api.Host.Tests
{
    public interface IPerson
    {
        System.Guid Id { get; }

        string Name { get; }

        System.DateTimeOffset? DateOfBirth { get; }

        string Group { get; }
    }
}
