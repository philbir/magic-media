using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace MagicMedia.Api.Host.Tests
{
    public interface IImageBox
    {
        int Left { get; }

        int Top { get; }

        int Right { get; }

        int Bottom { get; }
    }
}
