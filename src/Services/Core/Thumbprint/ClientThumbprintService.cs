using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;

namespace MagicMedia.Thumbprint
{
    public class ClientThumbprintService
    {
        public Task<string> GetOrCreate(ClientInfo clientInfo, CancellationToken cancellationToken)
        {
            




        }

        private string ComputeHash(ClientInfo clientInfo)
        {
            return $"{clientInfo.IPAdddress}_{clientInfo.UserAgent}".ComputeSha256(); 
        }
    }
}
