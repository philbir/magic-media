using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MagicMedia.Identity.Core.Services;

namespace MagicMedia.Identity.SignUp
{
    public class SignUpService 
    {
        private readonly ISmsService _smsService;

        public SignUpService(ISmsService smsService)
        {
            _smsService = smsService;
        }

        public async Task SendSmsCodeAsync(string mobile, CancellationToken cancellationToken)
        {
            var code = "123456";

            await _smsService.SendSmsAsync(mobile, $"Your Code: {code}", cancellationToken);
        }
    }
}
