using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia
{
    public class FolderTreeService : IFolderTreeService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IUserContextFactory _userContextFactory;

        public FolderTreeService(
            IMediaStore mediaStore,
            IUserContextFactory userContextFactory)
        {
            _mediaStore = mediaStore;
            _userContextFactory = userContextFactory;
        }


        public async Task<FolderItem> GetTreeAsync(CancellationToken cancellationToken)
        {
            IUserContext userContext = await _userContextFactory.CreateAsync(cancellationToken);

            IEnumerable<Guid>? ids = null;

            if (!userContext.HasPermission(Permissions.Media.ViewAll))
            {
                ids = await userContext.GetAuthorizedMediaAsync(cancellationToken);
            }

            IEnumerable<string> all = await _mediaStore.GetAllFoldersAsync(ids, cancellationToken);

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
            var prep = flat.Distinct(new FolderItemComparer()).OrderBy(x => x.Level).ThenBy(x => x.Path, new FolderComparer()).ToList();
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

    class FolderComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            var xNr = GetNumber(x);
            var yNr = GetNumber(y);

            if (xNr.HasValue)
            {
                if (yNr.HasValue)
                {
                    return yNr.Value.CompareTo(xNr);
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return x.CompareTo(y);
            }
        }


        private int? GetNumber(string? value)
        {
            if (int.TryParse(value, out var nr))
            {
                return nr;
            }
            return null;
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
