using ECommerceSaga.Inventory.Application.Interfaces;
using ECommerceSaga.Inventory.Infrastructure.Configuration;
using ECommerceSaga.Inventory.Infrastructure.Messaging;
using ECommerceSaga.Inventory.Infrastructure.Messaging.Consumer;
using ECommerceSaga.Inventory.Infrastructure.Messaging.Consumers;
using ECommerceSaga.Inventory.Infrastructure.Persistence;
using ECommerceSaga.Inventory.Infrastructure.Persistence.Reposirotires;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ECommerceSaga.Inventory.Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInventoryInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));

            var connectionString = configuration.GetConnectionString("InventoryDbConnection");
            services.AddDbContext<InventoryDbContext>(options =>
            {
                options.UseNpgsql(connectionString, pgSqlOptions =>
                {
                    pgSqlOptions.MigrationsAssembly(typeof(DependencyInjectionExtensions).Assembly.FullName);
                });
            });

            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

            services.AddMassTransit(configurator =>
            {
                configurator.AddConsumer<ReserveInventoryCommandConsumer>();
                configurator.AddConsumer<ReleaseInventoryCommandConsumer>();

                configurator.UsingRabbitMq((context, mqConfig) =>
                {
                    var rabbitMQOptions = context.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                    mqConfig.Host(rabbitMQOptions.HostName, "/", h =>
                    {
                        h.Username(rabbitMQOptions.UserName);
                        h.Password(rabbitMQOptions.Password);
                    });

                    mqConfig.ReceiveEndpoint("inventory-service", e =>
                    {
                        e.ConfigureConsumer<ReserveInventoryCommandConsumer>(context);
                        e.ConfigureConsumer<ReleaseInventoryCommandConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
