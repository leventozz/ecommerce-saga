using MediatR;

namespace ECommerceSaga.Order.Application.Features.CreateOrder
{
    public record SubmitOrderCommand : IRequest<Guid>
    {
        public Guid CustomerId { get; init; }
        public required List<OrderItemCommand> OrderItems { get; init; }
        public decimal TotalAmount { get; set; }
    }

    public record OrderItemCommand
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
