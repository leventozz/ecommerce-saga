using ECommerceSaga.Shared.Contracts.Common;
using MediatR;

namespace ECommerceSaga.Inventory.Application.Features.ReleaseInventory
{
    public record ReleaseInventoryStockCommand : IRequest
    {
        public Guid CorrelationId { get; init; }
        public List<OrderItem> OrderItems { get; init; }
    }
}
