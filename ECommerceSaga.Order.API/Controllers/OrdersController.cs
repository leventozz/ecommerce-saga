using ECommerceSaga.Order.API.Features.CreateOrder;
using ECommerceSaga.Order.Application.Features.CreateOrder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceSaga.Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var command = new SubmitOrderCommand
            {
                CustomerId = request.CustomerId,
                TotalAmount = request.TotalAmount,
                OrderItems = request.OrderItems
                    .Select(item => new OrderItemCommand
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }).ToList()
            };

            var orderId = await _mediator.Send(command, cancellationToken);

            return Accepted(new { OrderId = orderId });
        }
    }
}
