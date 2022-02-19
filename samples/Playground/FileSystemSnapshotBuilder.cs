using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MagicMedia.Playground
{
    public class FileSystemSnapshotBuilder
    {
        public static void BuildSnapshot()
        {
            var root = @"H:\Photos\Moments";

            var entries = new List<MediaFileEntry>();

            foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(file);

                var info = new MediaFileEntry
                {
                    Filename = fileInfo.Name,
                    Path = fileInfo.DirectoryName
                };

                entries.Add(info);
            }

            var json = JsonConvert.SerializeObject(entries);

            File.WriteAllText(@"c:\MagicMedia\FsTree.json", json);
        }

        public static IEnumerable<MediaFileEntry> Load()
        {
            var json = File.ReadAllText(@"c:\MagicMedia\FsTree.json");

            return JsonConvert.DeserializeObject<List<MediaFileEntry>>(json);
        }
    }

    public class MediaFileEntry
    {
        public string Filename { get; set; }

        public string Path { get; set; }
    }
}
