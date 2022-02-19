using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MagicMedia.Store;

public class MediaHash : IComparable<MediaHash>
{
    public MediaHashType Type { get; set; }

    public string? Value { get; set; }

    public int CompareTo(MediaHash? other)
    {
        return $"{Type}{Value}".CompareTo($"{other.Type}{other.Value}");
    }
}

public class MediaHashComparer : IEqualityComparer<MediaHash>
{
    public bool Equals(MediaHash? x, MediaHash? y)
    {
        return $"{x?.Type}{x?.Value}".Equals($"{y?.Type}{y?.Value}");
    }

    public int GetHashCode([DisallowNull] MediaHash obj)
    {
        return $"{obj.Type}{obj.Value}".GetHashCode();
    }
}
