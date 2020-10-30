using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace MagicMedia.Api.Host.Tests
{
    public interface IMedia
    {
        System.Guid Id { get; }

        string Filename { get; }

        System.DateTimeOffset? DateTaken { get; }

        IMediaDimension Dimension { get; }

        ICamera Camera { get; }

        IReadOnlyList<IMediaFace> Faces { get; }

        IMediaThumbnail Thumbnail { get; }
    }
}
