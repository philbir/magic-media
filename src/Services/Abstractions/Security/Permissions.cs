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

        public static class Album
        {
            public static readonly string ViewAll = "ALBUM_VIEW_ALL";
        }

        public static class Person
        {
            public static readonly string ViewAll = "PERSON_VIEW_ALL";
        }

        public static readonly Dictionary<string, List<string>> RoleMap;

        static Permissions()
        {
            RoleMap = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Admin"] = new List<string>
                {
                    Media.ViewAll,
                    Album.ViewAll,
                    Person.ViewAll
                }
            };
        }
    }
}
