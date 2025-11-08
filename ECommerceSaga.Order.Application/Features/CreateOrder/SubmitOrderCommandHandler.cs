using ECommerceSaga.Order.Application.Interfaces;
using ECommerceSaga.Shared.Contracts.Common;
using ECommerceSaga.Shared.Contracts.Order;
using MediatR;

namespace ECommerceSaga.Order.Application.Features.CreateOrder
{
    public class SubmitOrderCommandHandler : IRequestHandler<SubmitOrderCommand, Guid>
    {
        private readonly IEventPublisher _eventPublisher;

        public SubmitOrderCommandHandler(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task<Guid> Handle(SubmitOrderCommand request, CancellationToken cancellationToken)
        {
            var newOrderId = Guid.NewGuid();

            var orderSubmittedEvent = new OrderSubmittedEvent
            {
                OrderId = newOrderId,
                CustomerId = request.CustomerId,
                Timestamp = DateTime.UtcNow,
                TotalAmount = request.TotalAmount,
                OrderItems = request.OrderItems
                    .Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }).ToList()
            };

            await _eventPublisher.Publish(orderSubmittedEvent, cancellationToken);

            return newOrderId;
        }
    }
}
