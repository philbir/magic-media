using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Store.MongoDb
{
    internal static class CollectionNames
    {
        public static readonly string Media = "media";
        public static readonly string Face = "face";
        public static readonly string Camera = "camera";
        public static readonly string Album = "album";
        public static readonly string Person = "person";
        public static readonly string Group = "group";
        public static readonly string GeoAddressCache = "geoAddressCache";
        public static readonly string MediaOperation = "operation";
    }
}
