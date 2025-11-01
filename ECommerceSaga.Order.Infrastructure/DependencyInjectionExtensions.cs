using ECommerceSaga.Order.Application.Interfaces;
using ECommerceSaga.Order.Infrastructure.Configuration;
using ECommerceSaga.Order.Infrastructure.Messaging;
using ECommerceSaga.Order.Infrastructure.Persistence;
using ECommerceSaga.Order.Infrastructure.StateInstances;
using ECommerceSaga.Order.Infrastructure.StateMachines;
using ECommerceSaga.Shared.Contracts.Inventory;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ECommerceSaga.Order.Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddOrderInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("OrderStateDbConnection");
            services.Configure<RabbitMQOptions>(configuration.GetSection(RabbitMQOptions.SectionName));
            services.AddScoped<IEventPublisher, MassTransitEventPublisher>();
            EndpointConvention.Map<ReserveInventoryCommand>(new Uri("queue:inventory-service"));
            services.AddMassTransit(configurator =>
            {
                configurator.AddSagaStateMachine<OrderSagaStateMachine, OrderStateInstance>();
                configurator.SetEntityFrameworkSagaRepositoryProvider(repoConfig =>
                {
                    // lock db row
                    repoConfig.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                    repoConfig.UsePostgres();

                    repoConfig.AddDbContext<DbContext, OrderStateDbContext>((provider, options) =>
                    {
                        options.UseNpgsql(connectionString, pgSqlOptions =>
                        {
                            // for migration files
                            pgSqlOptions.MigrationsAssembly(
                                typeof(DependencyInjectionExtensions).Assembly.FullName);
                        });
                    });
                });

                configurator.UsingRabbitMq((context, mqConfig) =>
                {
                    var rabbitMQOptions = context.GetRequiredService<IOptions<RabbitMQOptions>>().Value;

                    mqConfig.Host(rabbitMQOptions.HostName, "/", h =>
                    {
                        h.Username(rabbitMQOptions.UserName);
                        h.Password(rabbitMQOptions.Password);
                    });
                    mqConfig.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
