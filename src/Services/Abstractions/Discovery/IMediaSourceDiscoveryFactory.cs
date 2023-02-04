using System.Collections.Generic;

namespace MagicMedia.Discovery;

public interface IMediaSourceDiscoveryFactory
{
    IMediaSourceDiscovery GetSource(MediaDiscoverySource type);
    IEnumerable<IMediaSourceDiscovery> GetSources();
}
