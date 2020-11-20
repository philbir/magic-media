using System.Collections.Generic;

namespace MagicMedia.Search
{
    public class SearchResult<TItem>
    {
        public int TotalCount { get; init; }

        public IEnumerable<TItem> Items { get; init; }

        public bool HasMore { get; init; }

        public SearchResult(IEnumerable<TItem> items, int totalCount)
        {
            TotalCount = totalCount;
            Items = items;
        }

        public SearchResult(IEnumerable<TItem> items, bool hasMore)
        {
            Items = items;
            HasMore = hasMore;
        }
    }
}
