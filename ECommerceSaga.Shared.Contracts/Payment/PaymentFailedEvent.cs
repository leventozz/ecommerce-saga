namespace ECommerceSaga.Shared.Contracts.Payment
{
    public class PaymentFailedEvent
    {
        public Guid CorrelationId { get; init; }
        public required string FaultReason { get; init; }
    }
}
