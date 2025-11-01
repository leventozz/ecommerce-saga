using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerceSaga.Order.Application
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
