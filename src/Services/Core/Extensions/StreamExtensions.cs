using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MagicMedia.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            if (stream is MemoryStream s)
            {
                return s.ToArray();
            }

            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);

            return ms.ToArray();
        }
    }
}
