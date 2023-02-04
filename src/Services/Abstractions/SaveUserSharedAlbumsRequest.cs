using System;
using System.Collections.Generic;

namespace MagicMedia;

public record SaveUserSharedAlbumsRequest(Guid UserId, IEnumerable<Guid> Albums);
