using ECommerceSaga.Inventory.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerceSaga.Inventory.Application.Features.ReleaseInventory
{
    public class ReleaseInventoryStockCommandHandler : IRequestHandler<ReleaseInventoryStockCommand>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<ReleaseInventoryStockCommandHandler> _logger;

        public ReleaseInventoryStockCommandHandler(IInventoryRepository inventoryRepository, ILogger<ReleaseInventoryStockCommandHandler> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task Handle(ReleaseInventoryStockCommand request, CancellationToken cancellationToken)
        {
            await _inventoryRepository.ReleaseStockAsync(request.CorrelationId,request.OrderItems);
        }
    }
}
