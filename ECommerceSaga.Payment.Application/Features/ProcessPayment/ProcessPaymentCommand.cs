using MediatR;

namespace ECommerceSaga.Payment.Application.Features.ProcessPayment
{
    public class ProcessPaymentCommand : IRequest
    {
        public Guid CorrelationId { get; init; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}
