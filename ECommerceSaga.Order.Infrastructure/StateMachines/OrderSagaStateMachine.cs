using ECommerceSaga.Order.Infrastructure.StateInstances;
using ECommerceSaga.Shared.Contracts.Order;
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
        public Event<InventoryReservedEvent> InventoryReserved { get; private set; }
        public Event<InventoryReservationFailedEvent> InventoryReservationFailed { get; private set; }

        //later
        // public Event<PaymentCompletedEvent> PaymentCompleted { get; private set; }
        // public Event<PaymentFailedEvent> PaymentFailed { get; private set; }

        #endregion

        #region Constructor

        public OrderSagaStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderSubmitted,
                config => config.CorrelateBy((instance, context) => instance.CorrelationId == context.Message.OrderId)
                                .SelectId(context => context.Message.OrderId)
                                .InsertOnInitial = true);

            Event(() => InventoryReserved,
                config => config.CorrelateById(context => context.Message.CorrelationId));

            Event(() => InventoryReservationFailed,
                config => config.CorrelateById(context => context.Message.CorrelationId));


            Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.CustomerId = context.Message.CustomerId;
                        context.Saga.CreatedDate = context.Message.Timestamp;
                    })
                    .TransitionTo(AwaitingInventory)
                    .Send(context => new ReserveInventoryCommand
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        OrderItems = context.Message.OrderItems
                    })
            );

            During(AwaitingInventory,
                When(InventoryReserved)
                    .Then(context =>
                    {
                        // loggind adn other actions
                    })
                    .TransitionTo(Completed), 

                When(InventoryReservationFailed)
                    .Then(context =>
                    {
                        // logging
                        context.Saga.FaultReason = context.Message.FaultReason;
                    })
                    .TransitionTo(Faulted)
            );
        }

        #endregion
    }
}
