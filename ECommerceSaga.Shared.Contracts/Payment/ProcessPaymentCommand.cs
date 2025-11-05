using ECommerceSaga.Shared.Contracts.Common;

namespace ECommerceSaga.Shared.Contracts.Payment
{
    public record ProcessPaymentCommand
    {
        public Guid CorrelationId { get; init; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}
