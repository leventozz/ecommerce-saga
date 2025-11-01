using ECommerceSaga.Shared.Contracts.Common;

namespace ECommerceSaga.Inventory.Infrastructure.Interfaces
{
    public interface IInventoryRepository
    {
        Task AddStockAsync(Guid productId, int quantity);

        Task<(bool IsSuccess, string FailReason)> ReserveStockAsync(Guid correlationId, List<OrderItem> items);

        // compensation
        Task ReleaseStockAsync(Guid correlationId, List<OrderItem> items);
    }
}
