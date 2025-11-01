using ECommerceSaga.Inventory.Infrastructure.Interfaces;
using ECommerceSaga.Shared.Contracts.Inventory;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ECommerceSaga.Inventory.Infrastructure.Consumers
{
    public class ReserveInventoryCommandConsumer : IConsumer<ReserveInventoryCommand>
    {
        private readonly ILogger<ReserveInventoryCommandConsumer> _logger;
        private readonly IInventoryRepository _inventoryRepository;

        public ReserveInventoryCommandConsumer(ILogger<ReserveInventoryCommandConsumer> logger, IInventoryRepository inventoryRepository)
        {
            _logger = logger;
            _inventoryRepository = inventoryRepository;
        }

        public async Task Consume(ConsumeContext<ReserveInventoryCommand> context)
        {
            var command = context.Message;

            _logger.LogInformation(
                "Saga { CorrelationId}: Reservation request received.Items: { ItemCount}",
                command.CorrelationId,
                command.OrderItems.Count);

            var (isSuccess, failReason) =
                await _inventoryRepository.ReserveStockAsync(
                    command.CorrelationId,
                    command.OrderItems);

            if (isSuccess)
            {
                var reservedEvent = new InventoryReservedEvent
                {
                    CorrelationId = command.CorrelationId
                };

                await context.Publish(reservedEvent);

                _logger.LogInformation(
                    "Saga {CorrelationId}: Reservation successful.",
                    command.CorrelationId);
            }
            else
            {
                var failedEvent = new InventoryReservationFailedEvent
                {
                    CorrelationId = command.CorrelationId,
                    FaultReason = failReason
                };

                await context.Publish(failedEvent);

                _logger.LogWarning(
                    "Saga {CorrelationId}: Reservation failed. Reason: {Reason}",
                    command.CorrelationId,
                    failReason);
            }
        }
    }
}
