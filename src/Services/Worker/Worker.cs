using MagicMedia.Processing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Worker
{
    public class Worker : BackgroundService
    {
        private readonly IMediaSourceScanner _mediaSourceScanner;

        public Worker(IMediaSourceScanner mediaSourceScanner)
        {
            _mediaSourceScanner = mediaSourceScanner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _mediaSourceScanner.ScanAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
