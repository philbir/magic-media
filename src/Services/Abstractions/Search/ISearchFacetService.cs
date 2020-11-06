﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;

namespace MagicMedia
{
    public interface ISearchFacetService
    {
        Task<IEnumerable<SearchFacetItem>> GetCityFacetsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<SearchFacetItem>> GetCountryFacetsAsync(CancellationToken cancellationToken);
    }
}