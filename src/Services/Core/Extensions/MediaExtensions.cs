using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public static class MediaExtensions
    {
        public static MediaBlobData ToBlobDataRequest(this Media media)
        {
            return new MediaBlobData
            {
                Type = MediaBlobType.Media,
                Directory = media.Folder,
                Filename = media.Filename
            };
        }
    }
}
