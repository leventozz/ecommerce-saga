using ECommerceSaga.Inventory.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSaga.Inventory.Infrastructure.Persistence
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options): base(options)
        {
        }

        public DbSet<InventoryItem> InventoryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InventoryItem>().HasData(
                new InventoryItem
                {
                    ProductId = Guid.Parse("a1b2c3d4-e5f6-4789-a012-345678901234"),
                    ProductName = "Product 1",
                    TotalStock = 50,
                    ReservedStock = 0
                },
                new InventoryItem
                {
                    ProductId = Guid.Parse("b2c3d4e5-f6a7-4890-b123-456789012345"),
                    ProductName = "Product 2",
                    TotalStock = 20,
                    ReservedStock = 0
                }
            );
        }
    }
}
