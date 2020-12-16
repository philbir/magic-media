using System;
using System.Collections.Generic;

namespace MagicMedia.Security
{
    public static class Permissions
    {
        public static class Media
        {
            public static readonly string ViewAll = "MEDIA_VIEW_ALL";
        }

        internal static readonly Dictionary<string, List<string>> RoleMap;

        static Permissions()
        {
            RoleMap = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Admin"] = new List<string>
                {
                    Media.ViewAll
                }
            };
        }
    }
}
