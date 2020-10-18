using System.Collections.Generic;

namespace MagicMedia.Discovery
{
    public class FileSystemDiscoveryOptions
    {
        public IEnumerable<string> Locations { get; set; } = new List<string>();
    }
}
