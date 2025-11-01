using ECommerceSaga.Inventory.Application.Interfaces;
using ECommerceSaga.Shared.Contracts.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace ECommerceSaga.Inventory.Infrastructure.Persistence.Reposirotires
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _context;
        private readonly ILogger<InventoryRepository> _logger;

        public InventoryRepository(InventoryDbContext context, ILogger<InventoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddStockAsync(Guid productId, int quantity)
        {
            var item = await _context.InventoryItems.FindAsync(productId);
            if (item != null)
            {
                item.TotalStock += quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReleaseStockAsync(Guid correlationId, List<OrderItem> items)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var requestedItem in items)
                {
                    var stockItem = await _context.InventoryItems.FindAsync(requestedItem.ProductId);
                    if (stockItem != null)
                    {
                        stockItem.ReservedStock -= requestedItem.Quantity;
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Saga {CorrelationId}: Stock release is complete.", correlationId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Saga {CorrelationId}: Critical error during stock release.", correlationId);
            }
        }

        public async Task<(bool IsSuccess, string FailReason)> ReserveStockAsync(Guid correlationId, List<OrderItem> items)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable); 

            try
            {
                foreach (var requestedItem in items)
                {
                    var stockItem = await _context.InventoryItems
                        .FromSqlInterpolated($@"SELECT * FROM ""InventoryItems"" WHERE ""ProductId"" = {requestedItem.ProductId} FOR UPDATE")
                        .FirstOrDefaultAsync();

                    if (stockItem == null)
                    {
                        return (false, $"Product not found: {requestedItem.ProductId}");
                    }

                    int availableStock = stockItem.TotalStock - stockItem.ReservedStock;

                    if (availableStock < requestedItem.Quantity)
                    {
                        return (false, $"Insufficient stock: {stockItem.ProductName}");
                    }

                    stockItem.ReservedStock += requestedItem.Quantity;
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Saga {CorrelationId}: The stock reservation transaction has been completed.", correlationId);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Saga {CorrelationId}: Critical error during stock reservation.", correlationId);
                return (false, $"Critical database error: {ex.Message}");
            }
        }
    }
}
