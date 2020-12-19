using System;
using System.Collections.Generic;

namespace MagicMedia.Security
{
    public static class Permissions
    {
        public static class General
        {
            public static readonly string Settings = "GENERAL_SETTINGS";
        }


        public static class Media
        {
            public static readonly string ViewAll = "MEDIA_VIEW_ALL";
            public static readonly string Edit = "MEDIA_EDIT";
        }

        public static class Face
        {
            public static readonly string ViewAll = "FACE_VIEW_ALL";

            public static readonly string Edit = "FACE_EDIT";
        }

        public static class Album
        {
            public static readonly string ViewAll = "ALBUM_VIEW_ALL";
            public static readonly string Edit = "ALBUM_EDIT";
        }

        public static class Person
        {
            public static readonly string ViewAll = "PERSON_VIEW_ALL";
            public static readonly string Edit = "PERSON_EDIT";
        }

        public static class User
        {
            public static readonly string Manage = "USER_MANAGE";
        }

        public static readonly Dictionary<string, List<string>> RoleMap;

        static Permissions()
        {
            RoleMap = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Admin"] = new List<string>
                {
                    Media.ViewAll,
                    Media.Edit,
                    Face.ViewAll,
                    Face.Edit,
                    Album.ViewAll,
                    Album.Edit,
                    Person.ViewAll,
                    Person.Edit,
                    General.Settings
                }
            };
        }
    }
}
