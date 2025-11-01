using System.ComponentModel.DataAnnotations;

namespace ECommerceSaga.Inventory.Infrastructure.Persistence.Entities
{
    public class InventoryItem
    {
        [Key]
        public Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public int TotalStock { get; set; }
        public int ReservedStock { get; set; }
    }
}
