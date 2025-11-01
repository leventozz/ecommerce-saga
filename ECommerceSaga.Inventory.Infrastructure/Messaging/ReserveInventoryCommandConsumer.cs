using ECommerceSaga.Inventory.Application.Features.ReserveInventory;
using ECommerceSaga.Shared.Contracts.Inventory;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerceSaga.Inventory.Infrastructure.Messaging
{
    public class ReserveInventoryCommandConsumer : IConsumer<ReserveInventoryCommand>
    {
        private readonly ILogger<ReserveInventoryCommandConsumer> _logger;
        private readonly IMediator _mediator;

        public ReserveInventoryCommandConsumer(ILogger<ReserveInventoryCommandConsumer> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ReserveInventoryCommand> context)
        {
            _logger.LogInformation(
                "Saga {CorrelationId}: (Consumer) Message received from the queue, being directed to MediatR.",
                context.Message.CorrelationId);

            var command = new ProcessInventoryReservationCommand
            {
                CorrelationId = context.Message.CorrelationId,
                OrderItems = context.Message.OrderItems
            };

            await _mediator.Send(command, context.CancellationToken);
        }
    }
}
