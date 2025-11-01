using ECommerceSaga.Order.Infrastructure.StateInstances;
using ECommerceSaga.Shared.Contracts;
using MassTransit;

namespace ECommerceSaga.Order.Infrastructure.StateMachines
{
    public class OrderSagaStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        #region States

        public State Submitted { get; private set; }
        public State AwaitingInventory { get; private set; }
        public State AwaitingPayment { get; private set; }
        public State Completed { get; private set; }
        public State Faulted { get; private set; } //error
        public State Cancelled { get; private set; }

        #endregion

        #region Events
        public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; }

        //later
        // public Event<InventoryReservedEvent> InventoryReserved { get; private set; }
        // public Event<InventoryReservationFailedEvent> InventoryReservationFailed { get; private set; }
        // public Event<PaymentCompletedEvent> PaymentCompleted { get; private set; }
        // public Event<PaymentFailedEvent> PaymentFailed { get; private set; }

        #endregion

        #region Constructor

        public OrderSagaStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.CustomerId = context.Message.CustomerId;
                        context.Saga.CreatedDate = context.Message.Timestamp;
                    })
                    .TransitionTo(Submitted)
            );
        }

        #endregion
    }
}
