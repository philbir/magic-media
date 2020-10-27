using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace MagicMedia.Api.Host.Tests
{
    public class MediaDetails
        : IMediaDetails
    {
        public MediaDetails(
            IMedia mediaById)
        {
            MediaById = mediaById;
        }

        public IMedia MediaById { get; }
    }
}
