namespace ECommerceSaga.Shared.Contracts.Common
{
    public record OrderItem
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
