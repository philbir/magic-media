using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace MagicMedia.Api.Host.Tests
{
    public class ImageBox
        : IImageBox
    {
        public ImageBox(
            int left, 
            int top, 
            int right, 
            int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int Left { get; }

        public int Top { get; }

        public int Right { get; }

        public int Bottom { get; }
    }
}
