using MagicMedia.Processing;
using MassTransit;
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
        private readonly IBusControl _busControl;

        public Worker(IMediaSourceScanner mediaSourceScanner, IBusControl busControl)
        {
            _mediaSourceScanner = mediaSourceScanner;
            _busControl = busControl;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _busControl.StartAsync();

            await _mediaSourceScanner.ScanAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
