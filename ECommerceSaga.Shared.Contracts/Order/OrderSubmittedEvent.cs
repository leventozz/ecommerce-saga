using ECommerceSaga.Shared.Contracts.Common;

namespace ECommerceSaga.Shared.Contracts.Order
{
    public record OrderSubmittedEvent
    {
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public DateTime Timestamp { get; init; }
        public required List<OrderItem> OrderItems { get; init; }
        public decimal TotalAmount { get; set; }
    }
}
