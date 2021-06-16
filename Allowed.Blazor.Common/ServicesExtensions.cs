using Allowed.Blazor.Common.Console;
using Allowed.Blazor.Common.Localization;
using Allowed.Blazor.Common.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Allowed.Blazor.Common
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddStorages(this IServiceCollection services)
        {
            services.AddScoped<StorageQueue>();
            services.AddTransient<LocalStorage>();
            services.AddTransient<CookieStorage>();

            return services;
        }

        public static IServiceCollection AddJSConsole(this IServiceCollection services)
        {
            services.AddTransient<ConsoleWriter>();

            return services;
        }

        public static IServiceCollection AddJSLocalization(this IServiceCollection services)
        {
            services.AddTransient<TimezoneHelper>();

            return services;
        }
    }
}
