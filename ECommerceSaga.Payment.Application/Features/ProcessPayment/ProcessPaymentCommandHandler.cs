using ECommerceSaga.Payment.Application.Interfaces;
using ECommerceSaga.Shared.Contracts.Payment;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerceSaga.Payment.Application.Features.ProcessPayment
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<ProcessPaymentCommandHandler> _logger;

        public ProcessPaymentCommandHandler(IEventPublisher eventPublisher, ILogger<ProcessPaymentCommandHandler> logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing payment for Saga:{CorrelationId} CustomerId: {CustomerId}, Amount: {Amount}", request.CorrelationId, request.CustomerId, request.Amount);

            //simulate payment processing

            await Task.Delay(2000, cancellationToken); 

            if (request.Amount < 5000m)
            {
                
                var completedEvent = new PaymentCompletedEvent
                {
                    CorrelationId = request.CorrelationId
                };
                await _eventPublisher.Publish(completedEvent, cancellationToken);
                _logger.LogInformation(
                    "Saga {CorrelationId}: Payment Success",
                    request.CorrelationId);
            }
            else
            {
                var failedEvent = new PaymentFailedEvent
                {
                    CorrelationId = request.CorrelationId,
                    FaultReason = "low budget"
                };
                await _eventPublisher.Publish(failedEvent, cancellationToken);
                _logger.LogWarning(
                    "Saga {CorrelationId}: Payment Fail. Reaseon: {Reason}",
                    request.CorrelationId,
                    failedEvent.FaultReason);
            }
        }
    }
}
