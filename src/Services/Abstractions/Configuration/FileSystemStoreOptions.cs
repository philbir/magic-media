using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Configuration
{
    public class FileSystemStoreOptions
    {
        public string RootDirectory { get; set; }

        public Dictionary<MediaBlobType, string> BlobTypeMap { get; set; }
    }
}
