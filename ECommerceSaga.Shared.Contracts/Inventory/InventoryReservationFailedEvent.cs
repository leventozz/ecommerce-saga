namespace ECommerceSaga.Shared.Contracts.Inventory
{
    public record InventoryReservationFailedEvent
    {
        public Guid CorrelationId { get; init; }
        public required string FaultReason { get; init; }
    }
}
