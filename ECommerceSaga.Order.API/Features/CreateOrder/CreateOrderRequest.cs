namespace ECommerceSaga.Order.API.Features.CreateOrder
{
    public record CreateOrderRequest
    {
        public Guid CustomerId { get; init; }
        public required List<OrderItemRequest> OrderItems { get; init; }
    }

    public record OrderItemRequest
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
