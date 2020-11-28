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
    public static class SchedulerServiceCollectionExtensions
    {

        public static IServiceCollection AddScheduler(
            this IServiceCollection services)
        {
            services.AddSingleton<StdSchedulerFactory>();
            services.AddSingleton<SchedulerAccessor>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IJobFactory, SingletonJobFactory>();

            return services;
        }
    }
}
