using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerceSaga.Inventory.Application
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInventoryApplication(
            this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
