using ECommerceSaga.Order.Infrastructure.StateInstances;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceSaga.Order.Infrastructure.Persistence
{
    public class OrderStateInstanceMap : SagaClassMap<OrderStateInstance>
    {
        protected override void Configure(EntityTypeBuilder<OrderStateInstance> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.CustomerId);

            entity.Property(x => x.OrderId);

            entity.Property(x => x.CreatedDate);

            entity.Property(x => x.FaultReason)
                .HasMaxLength(1024);
        }
    }
}
