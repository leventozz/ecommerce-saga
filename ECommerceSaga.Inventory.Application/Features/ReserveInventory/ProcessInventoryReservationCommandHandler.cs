using ECommerceSaga.Inventory.Application.Interfaces;
using ECommerceSaga.Shared.Contracts.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerceSaga.Inventory.Application.Features.ReserveInventory
{
    public class ProcessInventoryReservationCommandHandler : IRequestHandler<ProcessInventoryReservationCommand>
    {
        private readonly ILogger<ProcessInventoryReservationCommandHandler> _logger;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IEventPublisher _eventPublisher;

        public ProcessInventoryReservationCommandHandler(IEventPublisher eventPublisher, IInventoryRepository inventoryRepository, ILogger<ProcessInventoryReservationCommandHandler> logger)
        {
            _eventPublisher = eventPublisher;
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }
        public async Task Handle(ProcessInventoryReservationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Saga {CorrelationId}: Stock hold request received.",
                request.CorrelationId);

            var (isSuccess, failReason) =
                await _inventoryRepository.ReserveStockAsync(request.CorrelationId, request.OrderItems);

            if (isSuccess)
            {
                var reservedEvent = new InventoryReservedEvent
                {
                    CorrelationId = request.CorrelationId
                };

                await _eventPublisher.Publish(reservedEvent, cancellationToken);

                _logger.LogInformation(
                    "Saga {CorrelationId}: Stock hold request successfuk.",
                    request.CorrelationId);
            }
            else
            {
                var failedEvent = new InventoryReservationFailedEvent
                {
                    CorrelationId = request.CorrelationId,
                    FaultReason = failReason
                };

                await _eventPublisher.Publish(failedEvent, cancellationToken);

                _logger.LogWarning(
                    "Saga {CorrelationId}: Stock hold request failed. {Reason}",
                    request.CorrelationId,
                    failReason);
            }
        }
    }
}
