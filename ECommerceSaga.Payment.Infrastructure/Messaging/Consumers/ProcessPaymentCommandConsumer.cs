using ECommerceSaga.Payment.Application.Features.ProcessPayment;
using ECommerceSaga.Shared.Contracts.Payment;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerceSaga.Payment.Infrastructure.Messaging.Consumers
{
    public class ProcessPaymentCommandConsumer : IConsumer<ProcessPaymentCommandContract>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessPaymentCommandConsumer> _logger;

        public ProcessPaymentCommandConsumer(IMediator mediator, ILogger<ProcessPaymentCommandConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProcessPaymentCommandContract> context)
        {
            _logger.LogInformation(
                "Saga {CorrelationId}: (Consumer) Payment command received, being directed to MediatR.",
                context.Message.CorrelationId);

            var command = new ProcessPaymentCommand
            {
                CorrelationId = context.Message.CorrelationId,
                CustomerId = context.Message.CustomerId,
                Amount = context.Message.Amount
            };

            await _mediator.Send(command, context.CancellationToken);
        }
    }
}
