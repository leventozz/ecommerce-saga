namespace ECommerceSaga.Shared.Contracts
{
    public record OrderSubmittedEvent
    {
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public DateTime Timestamp { get; init; }
        public required List<OrderItem> OrderItems { get; init; }
    }

    public record OrderItem
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
