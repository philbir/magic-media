using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace MagicMedia.Scheduling
{
    public class SingletonJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public SingletonJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob ??
                throw new InvalidOperationException();
        }

        public void ReturnJob(IJob job)
        {
            // Method intentionally left empty.
        }
    }

    public class SchedulerAccessor
    {
        private readonly StdSchedulerFactory _factory;

        public SchedulerAccessor(StdSchedulerFactory factory)
        {
            _factory = factory;
        }

        public IScheduler GetScheduler() => _factory.GetScheduler().GetAwaiter().GetResult();
    }
}
