using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace MagicMedia.Api.Host.Tests
{
    public interface IMediaDimension
    {
        int Height { get; }

        int Width { get; }
    }
}
