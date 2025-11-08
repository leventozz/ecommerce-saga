using ECommerceSaga.Payment.Application.Interfaces;
using ECommerceSaga.Payment.Infrastructure.Configuration;
using ECommerceSaga.Payment.Infrastructure.Messaging;
using ECommerceSaga.Payment.Infrastructure.Messaging.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ECommerceSaga.Payment.Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddPaymentInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));


            services.AddScoped<IEventPublisher, MassTransitEventPublisher>();


            services.AddMassTransit(configurator =>
            {
                configurator.AddConsumer<ProcessPaymentCommandConsumer>();
                configurator.UsingRabbitMq((context, mqConfig) =>
                {
                    var rabbitMQOptions = context.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                    mqConfig.Host(rabbitMQOptions.HostName, "/", h =>
                    {
                        h.Username(rabbitMQOptions.UserName);
                        h.Password(rabbitMQOptions.Password);
                    });
                    mqConfig.ReceiveEndpoint("payment-service", e =>
                    {
                        e.ConfigureConsumer<ProcessPaymentCommandConsumer>(context);
                    });
                });
            });
            return services;
        }
    }
}
