using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace MagicMedia.Api.Host.Tests
{
    public interface IMediaFace
    {
        System.Guid Id { get; }

        IImageBox Box { get; }

        IMediaThumbnail1 Thumbnail { get; }

        IPerson Person { get; }

        System.Guid? PersonId { get; }
    }
}
