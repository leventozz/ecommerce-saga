using ECommerceSaga.Shared.Contracts.Common;

namespace ECommerceSaga.Shared.Contracts.Inventory
{
    public class ReleaseInventoryCommand
    {
        public Guid CorrelationId { get; init; }
        public required List<OrderItem> OrderItems { get; init; }
    }
}
