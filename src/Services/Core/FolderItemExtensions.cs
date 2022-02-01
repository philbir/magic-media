using System.Linq;

namespace MagicMedia;

public static class FolderItemExtensions
{

    public static FolderItem? FindNode(this FolderItem root, string path)
    {
        if (root.Path == path)
        {
            return root;
        }
        if (root.Children?.Count() > 0)
        {
            foreach (FolderItem? node in root.Children)
            {
                FolderItem? match = FindNode(node, path);
                if (match != null)
                {
                    return match;
                }
            }
        }

        return null;
    }

    public static string GetParentPath(this FolderItem root)
    {
        var frags = root.Path.Split('/');
        if (frags.Count() == 1)
        {
            return "/";
        }
        else
        {
            return string.Join("/", frags.Take(frags.Count() - 1));
        }
    }
}
