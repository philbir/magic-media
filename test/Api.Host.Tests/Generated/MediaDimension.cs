﻿namespace MagicMedia.Api.Host.Tests
{
    public class MediaDimension
        : IMediaDimension
    {
        public MediaDimension(
            int height, 
            int width)
        {
            Height = height;
            Width = width;
        }

        public int Height { get; }

        public int Width { get; }
    }
}
