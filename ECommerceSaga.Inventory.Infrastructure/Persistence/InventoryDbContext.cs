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
                    ProductId = new Guid(),
                    ProductName = "Product 1",
                    TotalStock = 50,
                    ReservedStock = 0
                },
                new InventoryItem
                {
                    ProductId = new Guid(),
                    ProductName = "Product 2",
                    TotalStock = 20,
                    ReservedStock = 0
                }
            );
        }
    }
}
