namespace ECommerceSaga.Shared.Contracts.Inventory
{
    public record InventoryReservedEvent
    {
        public Guid CorrelationId { get; init; }
    }
}
