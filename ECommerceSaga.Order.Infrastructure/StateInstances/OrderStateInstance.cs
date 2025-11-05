using MassTransit;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceSaga.Order.Infrastructure.StateInstances
{
    public class OrderStateInstance : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CurrentState { get; set; } = string.Empty;
        public Guid? CustomerId { get; set; }
        public Guid? OrderId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FaultReason { get; set; } = string.Empty;
        public decimal? TotalAmount { get; set; }
        public string OrderItemJson { get; set; } = string.Empty;
    }
}
