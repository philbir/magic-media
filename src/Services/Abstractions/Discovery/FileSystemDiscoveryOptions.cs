using System.Collections.Generic;

namespace MagicMedia.Discovery
{
    public class FileSystemDiscoveryOptions
    {
        public IEnumerable<FileDiscoveryLocation> Locations { get; set; }
            = new List<FileDiscoveryLocation>();
    }
}
