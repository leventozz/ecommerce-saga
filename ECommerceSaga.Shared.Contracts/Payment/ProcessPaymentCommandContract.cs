namespace ECommerceSaga.Shared.Contracts.Payment
{
    public record ProcessPaymentCommandContract
    {
        public Guid CorrelationId { get; init; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}
