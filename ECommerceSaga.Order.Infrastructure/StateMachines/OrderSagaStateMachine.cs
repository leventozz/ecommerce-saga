using ECommerceSaga.Order.Infrastructure.StateInstances;
using ECommerceSaga.Shared.Contracts.Common;
using ECommerceSaga.Shared.Contracts.Inventory;
using ECommerceSaga.Shared.Contracts.Order;
using ECommerceSaga.Shared.Contracts.Payment;
using MassTransit;
using System.Text.Json;

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
        public Event<PaymentCompletedEvent> PaymentCompleted { get; private set; }
        public Event<PaymentFailedEvent> PaymentFailed { get; private set; }

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

            Event(() => PaymentCompleted,
                config => config.CorrelateById(context => context.Message.CorrelationId));

            Event(() => PaymentFailed,
                config => config.CorrelateById(context => context.Message.CorrelationId));

            #region Saga Start

            Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.CustomerId = context.Message.CustomerId;
                        context.Saga.CreatedDate = context.Message.Timestamp;
                        context.Saga.TotalAmount = context.Message.TotalAmount;
                        context.Saga.OrderItemJson = JsonSerializer.Serialize(context.Message.OrderItems);
                    })
                    .TransitionTo(AwaitingInventory)
                    .Send(context => new ReserveInventoryCommandContract
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        OrderItems = context.Message.OrderItems
                    })
            );

            #endregion

            #region Inventory Flow

            During(AwaitingInventory,
                When(InventoryReserved)
                    .Then(context =>
                    {

                    })
                    .TransitionTo(AwaitingPayment)
                    .Send(context => new ProcessPaymentCommandContract
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        CustomerId = context.Saga.CustomerId ?? Guid.Empty,
                        Amount = context.Saga.TotalAmount ?? 0m
                    }),

                When(InventoryReservationFailed)
                    .Then(context =>
                    {
                        // logging
                        context.Saga.FaultReason = context.Message.FaultReason;
                    })
                    .TransitionTo(Faulted)
            );

            #endregion

            #region Payment Flow

            During(AwaitingPayment,
                When(PaymentCompleted)
                    .Then(context =>
                    {
                        // logging
                        // happy path
                    })
                    .TransitionTo(Completed)
                    .Finalize(),

                When(PaymentFailed)
                    .Then(context =>
                    {
                        // logging
                        context.Saga.FaultReason = context.Message.FaultReason;
                    })
                    .TransitionTo(Cancelled)
                    .Send(context => new ReleaseInventoryCommandContract
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        OrderItems = JsonSerializer.Deserialize<List<OrderItem>>(context.Saga.OrderItemJson)!
                    })
            );

            #endregion


            SetCompletedWhenFinalized();
        }

        #endregion
    }
}
