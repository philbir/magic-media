using System;
using System.IO;
using System.Linq;

namespace MagicMedia.TestLibrary
{
    public static class TestMediaLibrary
    {
        public static Stream WithExif => GetMedia("001.jpg");

        public static Stream TwoFacesNoExif => GetMedia("002.jpg");

        public static Stream TwoFacesWithExif => GetMedia("003.jpg");

        public static Stream SamsungFrameFormat => GetMedia("004.jpg");

        private static Stream GetMedia(string name)
        {
            var stream = new FileStream($"{GetTestMediaPath()}/{name}", FileMode.Open);

            return stream;
        }

        private static string GetTestMediaPath()
        {
            string[] segments = typeof(TestMediaLibrary).Assembly.Location.Split(Path.DirectorySeparatorChar);
            var idx = Array.IndexOf(segments, "magic-media");
            var root = string.Join(Path.DirectorySeparatorChar, segments.Take(idx + 1));

            return Path.Join(root, "test_media");
        }
    }
}
