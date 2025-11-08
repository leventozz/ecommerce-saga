using ECommerceSaga.Inventory.Application.Features.ReleaseInventory;
using ECommerceSaga.Shared.Contracts.Inventory;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerceSaga.Inventory.Infrastructure.Messaging.Consumer
{
    public class ReleaseInventoryCommandConsumer : IConsumer<ReleaseInventoryCommandContract>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReleaseInventoryCommandConsumer> _logger;

        public ReleaseInventoryCommandConsumer(IMediator mediator, ILogger<ReleaseInventoryCommandConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReleaseInventoryCommandContract> context)
        {
            var command = new ReleaseInventoryStockCommand
            {
                CorrelationId = context.Message.CorrelationId,
                OrderItems = context.Message.OrderItems
            };

            await _mediator.Send(command, context.CancellationToken);
        }
    }
}
