using MagicMedia.Identity.Services;
using MagicMedia.Identity.Services.Sms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Identity
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityCore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IUserAccountService, UserAccountService>();

            UserAccountOptions accountOptions = configuration.GetSection("Identity:Account")
                .Get<UserAccountOptions>();

            services.AddSingleton(accountOptions);

            services.AddSingleton<IUserFactory, UserFactory>();
            services.AddSingleton<ITotpCodeService, TotpCodeService>();
            services.AddECallSms(configuration);

            return services;
        }

    }
}
