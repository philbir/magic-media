using System;
using System.Collections.Generic;

namespace MagicMedia.Store;

public class Album
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public List<string>? Tags { get; set; }

    public Guid? CoverMediaId { get; set; }

    public List<AlbumInclude> Includes { get; set; } = new List<AlbumInclude>();

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public IEnumerable<AlbumPerson> Persons { get; set; } = new List<AlbumPerson>();

    public IEnumerable<AlbumCountry> Countries { get; set; } = new List<AlbumCountry>();

    public int ImageCount { get; set; }

    public int VideoCount { get; set; }

    public IEnumerable<Guid>? SharedWith { get; set; } = new List<Guid>();
}

public class AlbumPerson
{
    public Guid PersonId { get; set; }

    public string? Name { get; set; }

    public int Count { get; set; }

    public Guid FaceId { get; set; }
}

public class AlbumCountry
{
    public string? Name { get; set; }

    public int Count { get; set; }

    public IEnumerable<AlbumCity> Cities { get; set; } = new List<AlbumCity>();
    public string? Code { get; set; }
}

public class AlbumCity
{
    public string? Name { get; set; }

    public int Count { get; set; }
}

public enum AlbumIncludeType
{
    Ids = 1,
    Folder = 2,
    Query = 3
}

public class AlbumInclude
{
    public AlbumIncludeType Type { get; set; }

    public IEnumerable<Guid>? MediaIds { get; set; } = new List<Guid>();

    public IEnumerable<string>? Folders { get; set; } = new List<string>();

    public IEnumerable<FilterDescription>? Filters { get; set; }

    public string? SerializedQuery { get; set; }
}

public class FilterDescription
{
    public string? Key { get; set; }

    public string? Name { get; set; }

    public string? Value { get; set; }

    public string? Description { get; set; }
}
