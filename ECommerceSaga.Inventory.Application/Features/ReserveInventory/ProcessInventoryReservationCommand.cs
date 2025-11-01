using ECommerceSaga.Shared.Contracts.Common;
using MediatR;

namespace ECommerceSaga.Inventory.Application.Features.ReserveInventory
{
    public record ProcessInventoryReservationCommand : IRequest
    {
        public Guid CorrelationId { get; init; }
        public required List<OrderItem> OrderItems { get; init; }
    }
}
