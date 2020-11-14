using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Store
{
    public class Album
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public List<string>? Tags { get; set; }

        public string? CoverPhotoId { get; set; }

        public List<AlbumInclude> Includes { get; set; } = new List<AlbumInclude>();

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public List<AlbumPerson>? Persons { get; set; }

        public int PhotoCount { get; set; }

        public int VideoCount { get; set; }
    }

    public class AlbumPerson
    {
        public string PersonId { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }

        public string FaceId { get; set; }
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

        public IEnumerable<Guid> MediaIds { get; set; } = new List<Guid>();

        public List<string>? Folders { get; set; } = new List<string>();

        public string? SerializedQuery { get; set; }

    }
}
