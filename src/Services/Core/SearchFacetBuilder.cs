using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia
{
    public class SearchFacetBuilder
    {
        private readonly IMediaStore _mediaStore;

        public SearchFacetBuilder(IMediaStore mediaStore)
        {
            _mediaStore = mediaStore;
        }

        public async Task<IEnumerable<SearchFacetItem>> BuildCountryFacetsAsync()
        {
            return null;
        }
    }
}
