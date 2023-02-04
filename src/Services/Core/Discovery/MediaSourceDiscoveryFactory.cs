using System.Collections.Generic;
using System.Linq;

namespace MagicMedia.Discovery;

public class MediaSourceDiscoveryFactory : IMediaSourceDiscoveryFactory
{
    private readonly IEnumerable<IMediaSourceDiscovery> _mediaSources;

    public MediaSourceDiscoveryFactory(IEnumerable<IMediaSourceDiscovery> mediaSources)
    {
        _mediaSources = mediaSources;
    }

    public IEnumerable<IMediaSourceDiscovery> GetSources()
    {
        return _mediaSources;
    }

    public IMediaSourceDiscovery GetSource(MediaDiscoverySource type)
    {
        return _mediaSources.Single(x => x.SourceType == type);
    }
}
