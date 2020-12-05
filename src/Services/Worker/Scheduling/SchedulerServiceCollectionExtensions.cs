using System.Collections.Generic;
using MagicMedia.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace MagicMedia.Scheduling
{
    public static class SchedulerServiceCollectionExtensions
    {
        public static IMagicMediaServerBuilder AddScheduler(
            this IMagicMediaServerBuilder builder)
        {
            builder.Services.AddScheduler(builder.Configuration);

            return builder;
        }


        private static IServiceCollection AddScheduler(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<StdSchedulerFactory>();
            services.AddSingleton<SchedulerAccessor>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IJobFactory, SingletonJobFactory>();

            IEnumerable<JobScheduleOptions> schedules = configuration
                .GetSection("MagicMedia:JobSchedules")
                .Get<IEnumerable<JobScheduleOptions>>();

            services.AddSingleton(schedules);

            return services;
        }
    }
}
