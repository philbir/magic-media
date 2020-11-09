using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public class FolderTreeService : IFolderTreeService
    {
        private readonly IMediaStore _mediaStore;

        public FolderTreeService(IMediaStore mediaStore)
        {
            _mediaStore = mediaStore;
        }


        public async Task<FolderItem> GetTreeAsync(CancellationToken cancellationToken)
        {
            IEnumerable<string> all = await _mediaStore.GetAllFoldersAsync(cancellationToken);

            List<FolderItem> flat = GetFlatList(all);
            FolderItem root = BuildStructure(flat);

            return root;
        }

        private List<FolderItem> GetFlatList(IEnumerable<string> paths)
        {
            return paths.OrderBy(x => x).Select(GetItem).ToList();
        }

        private FolderItem GetItem(string path)
        {
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            path = path.TrimStart('/');

            return new FolderItem
            {
                Name = parts.Last(),
                Path = path,
                Level = parts.Length - 1,
                Children = new List<FolderItem>()
            };
        }

        private FolderItem BuildStructure(IEnumerable<FolderItem> flat)
        {
            var prep = flat.Distinct(new FolderItemComparer()).OrderBy(x => x.Level).ThenBy(x => x.Path).ToList();
            var root = new FolderItem { Path = "/", Name = "Home" };

            foreach (FolderItem? item in prep)
            {
                FolderItem? node = root.FindNode(item.Path);
                var frags = item.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < frags.Count(); i++)
                {
                    var virtFolder = new FolderItem()
                    {
                        Name = frags[i],
                        Path = string.Join("/", frags.Take(i + 1)),
                        Level = i + 1
                    };

                    var parentPath = virtFolder.GetParentPath();
                    FolderItem? parentNode = root.FindNode(parentPath);

                    if (!parentNode.Children.Any(x => x.Name == virtFolder.Name))
                    {
                        parentNode.Children.Add(virtFolder);
                    }
                }
            }

            return root;
        }
    }

    class FolderItemComparer : IEqualityComparer<FolderItem>
    {
        public bool Equals(FolderItem x, FolderItem y)
        {
            return x.Path == y.Path;
        }

        public int GetHashCode(FolderItem obj)
        {
            return obj.Path.GetHashCode();
        }
    }
}
