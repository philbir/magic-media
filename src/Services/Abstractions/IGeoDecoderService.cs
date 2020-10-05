using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia
{
    public interface IGeoDecoderService
    {
        Task<GeoAddress> DecodeAsync(
            double latitude, 
            double longitude, 
            CancellationToken cancellationToken);
    }
}
