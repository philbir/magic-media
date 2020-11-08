using System.Collections.Generic;

namespace MagicMedia.Search
{
    public class SearchResult<TItem>
    {
        public int TotalCount { get; init; }

        public IEnumerable<TItem> Items { get; init; }

        public SearchResult(IEnumerable<TItem> items, int totalCount)
        {
            TotalCount = totalCount;
            Items = items;
        }
    }
}
