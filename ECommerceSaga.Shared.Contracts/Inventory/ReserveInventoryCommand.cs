using ECommerceSaga.Shared.Contracts.Common;

namespace ECommerceSaga.Shared.Contracts.Inventory
{
    public record ReserveInventoryCommand
    {
        public Guid CorrelationId { get; init; }
        public required List<OrderItem> OrderItems { get; init; }
    }
}
