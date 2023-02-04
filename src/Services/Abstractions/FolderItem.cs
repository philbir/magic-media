using System.Collections.Generic;

namespace MagicMedia;

public class FolderItem
{
    public string? Name { get; set; }

    public string? Path { get; set; }

    public int Level { get; set; }

    public IList<FolderItem> Children { get; set; } = new List<FolderItem>();
}
