namespace ECommerceSaga.Shared.Contracts.Payment
{
    public class PaymentCompletedEvent
    {
        public Guid CorrelationId { get; init; }
    }
}
