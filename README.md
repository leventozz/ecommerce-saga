# ECommerce Saga Pattern Demo

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![MassTransit](https://img.shields.io/badge/MassTransit-8.5.5-FF6B6B)](https://masstransit.io/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-18-336791?logo=postgresql)](https://www.postgresql.org/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-4.2.0-FF6600?logo=rabbitmq)](https://www.rabbitmq.com/)

A production-ready demonstration of the **Saga Pattern** for orchestrating distributed transactions in a microservices architecture. This project showcases how to manage long-running business processes across multiple services using event-driven communication and state machines.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Saga Flow](#saga-flow)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Testing the Saga](#testing-the-saga)
- [Key Features](#key-features)
- [Architecture Decisions](#architecture-decisions)

## Overview

This project demonstrates a distributed e-commerce order processing system where an order must coordinate multiple services:

- **Order Service**: Orchestrates the entire order process using a state machine
- **Inventory Service**: Manages product inventory and reservations
- **Payment Service**: Handles payment processing

The Saga Pattern ensures data consistency across these services without using distributed transactions (2PC), which are often impractical in microservices architectures. Instead, the saga orchestrates a series of local transactions with compensation logic to handle failures.

### Why Saga Pattern?

In distributed systems, traditional ACID transactions are not feasible across service boundaries. The Saga Pattern provides:

- **Eventual Consistency**: Services eventually reach a consistent state
- **Compensation**: Automatic rollback through compensating transactions
- **Resilience**: Handles partial failures gracefully
- **Scalability**: No distributed locks or blocking operations

## Architecture

### Clean Architecture

The project follows **Clean Architecture** principles with clear separation of concerns

### Service Boundaries

Each service is independently deployable and maintains its own database:

- **ECommerceSaga.Order.API**: Orchestrates the saga using MassTransit state machine
- **ECommerceSaga.Inventory.API**: Manages inventory reservations and releases
- **ECommerceSaga.Payment.API**: Processes payments
- **ECommerceSaga.Shared.Contracts**: Shared message contracts for inter-service communication

### Communication Pattern

Services communicate asynchronously via **RabbitMQ** using **MassTransit**:

- **Commands**: Sent from Order Service to Inventory/Payment Services
- **Events**: Published by Inventory/Payment Services back to Order Service
- **State Machine**: Reacts to events and orchestrates the saga flow

## Technology Stack

### Core Technologies

- **.NET 8.0**: Latest LTS version with modern C# features
- **C# 12**: Primary constructors, file-scoped namespaces, record structs
- **ASP.NET Core**: Web API framework

### Messaging & Orchestration

- **MassTransit 8.5.5**: Enterprise service bus for .NET
  - State machine support for saga orchestration
  - RabbitMQ transport
  - Entity Framework Core persistence
- **RabbitMQ 4.2.0**: Message broker with management UI

### Data Persistence

- **Entity Framework Core**: ORM with PostgreSQL provider
- **Npgsql.EntityFrameworkCore.PostgreSQL 9.0.4**: PostgreSQL provider
- **PostgreSQL 18**: Relational database

### Patterns & Libraries

- **MediatR 13.1.0**: Mediator pattern for CQRS implementation
- **Docker Compose**: Infrastructure orchestration

## Project Structure

```
ECommerceSaga/
├── ECommerceSaga.Order.API/              # Order service API
│   ├── Controllers/                       # REST API endpoints
│   ├── Features/                         # Feature-based organization
│   └── Program.cs                        # Service startup
│
├── ECommerceSaga.Order.Application/       # Order business logic
│   ├── Features/                         # Use cases (CQRS)
│   │   └── CreateOrder/
│   └── Interfaces/                       # Application contracts
│
├── ECommerceSaga.Order.Infrastructure/   # Order infrastructure
│   ├── StateMachines/                    # Saga state machine
│   │   └── OrderSagaStateMachine.cs
│   ├── StateInstances/                   # Saga state persistence
│   ├── Messaging/                        # Event publishers
│   ├── Persistence/                      # DbContext, repositories
│   └── Migrations/                       # EF Core migrations
│
├── ECommerceSaga.Inventory.API/          # Inventory service API
├── ECommerceSaga.Inventory.Application/  # Inventory business logic
├── ECommerceSaga.Inventory.Infrastructure/# Inventory infrastructure
│   └── Messaging/
│       └── Consumers/                    # Command consumers
│
├── ECommerceSaga.Payment.API/            # Payment service API
├── ECommerceSaga.Payment.Application/    # Payment business logic
├── ECommerceSaga.Payment.Infrastructure/ # Payment infrastructure
│   └── Messaging/
│       └── Consumers/                    # Command consumers
│
├── ECommerceSaga.Shared.Contracts/       # Shared message contracts
│   ├── Order/                           # Order events
│   ├── Inventory/                       # Inventory commands/events
│   ├── Payment/                         # Payment commands/events
│   └── Common/                          # Shared DTOs
│
└── docker-compose.yml                    # Infrastructure setup
```

### Layer Responsibilities

**API Layer**:
- HTTP endpoints
- Request/Response DTOs
- Input validation
- Swagger documentation

**Application Layer**:
- Business logic and use cases
- CQRS with MediatR
- Feature-based organization
- Application interfaces

**Infrastructure Layer**:
- Database access (EF Core)
- Message bus (MassTransit)
- External service integrations
- State machine implementation

## Saga Flow

### State Machine States

The Order Saga progresses through the following states:

1. **Initial** → Order submitted
2. **AwaitingInventory** → Waiting for inventory reservation
3. **AwaitingPayment** → Inventory reserved, waiting for payment
4. **Completed** → All steps successful
5. **Faulted** → Inventory reservation failed
6. **Cancelled** → Payment failed, inventory released

### Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                         Order Submitted                          │
│                    (OrderSubmittedEvent)                          │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │  Awaiting       │
                    │  Inventory      │
                    └────────┬────────┘
                             │
                ┌────────────┴────────────┐
                │                         │
                ▼                         ▼
    ┌──────────────────────┐   ┌──────────────────────┐
    │ ReserveInventory     │   │ InventoryReservation │
    │ Command              │   │ Failed Event         │
    └──────────┬───────────┘   └──────────┬───────────┘
                │                          │
                │                          ▼
                │                  ┌──────────────┐
                │                  │   Faulted    │
                │                  └──────────────┘
                │
                ▼
    ┌──────────────────────┐
    │ InventoryReserved    │
    │ Event                │
    └──────────┬───────────┘
                │
                ▼
        ┌───────────────┐
        │  Awaiting     │
        │  Payment      │
        └───────┬───────┘
                │
    ┌───────────┴───────────┐
    │                       │
    ▼                       ▼
┌──────────────┐   ┌──────────────────┐
│ Process      │   │ PaymentFailed    │
│ Payment      │   │ Event            │
│ Command      │   └────────┬─────────┘
└──────┬───────┘            │
       │                    │
       │                    ▼
       │            ┌──────────────────┐
       │            │ ReleaseInventory │
       │            │ Command          │
       │            └────────┬─────────┘
       │                     │
       │                     ▼
       │            ┌──────────────┐
       │            │  Cancelled │
       │            └────────────┘
       │
       ▼
┌──────────────┐
│ Payment      │
│ Completed    │
│ Event        │
└──────┬───────┘
       │
       ▼
┌──────────────┐
│  Completed   │
└──────────────┘
```

### Compensation Logic

When payment fails, the saga automatically triggers compensation:

1. **Payment Failed Event** received
2. **Transition to Cancelled** state
3. **Release Inventory Command** sent to Inventory Service
4. Previously reserved inventory is released

This ensures that resources are not held indefinitely when the saga cannot complete successfully.

## Prerequisites

Before running the application, ensure you have the following installed:

- **.NET 8.0 SDK** or later
  - Download from [.NET Downloads](https://dotnet.microsoft.com/download)
  - Verify installation: `dotnet --version`
  
- **Docker Desktop** (for Windows/Mac) or **Docker Engine** (for Linux)
  - Download from [Docker Desktop](https://www.docker.com/products/docker-desktop)
  - Verify installation: `docker --version` and `docker-compose --version`

- **IDE** (optional but recommended):
  - Visual Studio 2022 (17.8 or later)
  - Visual Studio Code with C# extension
  - JetBrains Rider

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd ECommerceSaga
```

### 2. Start Infrastructure Services

Start PostgreSQL and RabbitMQ using Docker Compose:

```bash
docker-compose up -d
```

This will start:
- **PostgreSQL 18** on port `5432`
  - Database: `docker-db`
  - Username: `localuser`
  - Password: `myStrongPassword123!`
  
- **PgAdmin 4** on port `5050`
  - Email: `admin@example.com`
  - Password: `myStrongPassword123!`
  
- **RabbitMQ** on ports `5672` (AMQP) and `15672` (Management UI)
  - Username: `guest`
  - Password: `guest`
  - Management UI: http://localhost:15672

### 3. Verify Infrastructure

- **PostgreSQL**: Connect using any PostgreSQL client or PgAdmin
- **RabbitMQ Management**: Open http://localhost:15672 in your browser

### 4. Run Database Migrations

Each service has its own database context. Run migrations for all services:

```bash
# Order Service (Saga State)
cd ECommerceSaga.Order.Infrastructure
dotnet ef database update --project ../ECommerceSaga.Order.Infrastructure --startup-project ../ECommerceSaga.Order.API

# Inventory Service
cd ../ECommerceSaga.Inventory.Infrastructure
dotnet ef database update --project ../ECommerceSaga.Inventory.Infrastructure --startup-project ../ECommerceSaga.Inventory.API

# Payment Service (if it has a database)
# Note: Payment service may not require a database in this demo
```

### 5. Configure Connection Strings

Connection strings are already configured in `appsettings.Development.json` files for each service. They point to the Docker PostgreSQL instance:

```json
{
  "ConnectionStrings": {
    "OrderStateDbConnection": "Host=localhost;Port=5432;Database=docker-db;Username=localuser;Password=myStrongPassword123!",
    "InventoryDbConnection": "Host=localhost;Port=5432;Database=docker-db;Username=localuser;Password=myStrongPassword123!"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest"
  }
}
```

### 6. Start the Services

Open multiple terminal windows and start each service:

**Terminal 1 - Order Service:**
```bash
cd ECommerceSaga.Order.API
dotnet run
```
Service will run on: `https://localhost:7xxx` (check launchSettings.json)

**Terminal 2 - Inventory Service:**
```bash
cd ECommerceSaga.Inventory.API
dotnet run
```

**Terminal 3 - Payment Service:**
```bash
cd ECommerceSaga.Payment.API
dotnet run
```

Alternatively, you can run all services from Visual Studio by setting multiple startup projects.

### 7. Access Swagger Documentation

Once services are running, access Swagger UI:

- **Order Service**: `https://localhost:7xxx/swagger`
- **Inventory Service**: `https://localhost:7xxx/swagger`
- **Payment Service**: `https://localhost:7xxx/swagger`

## API Endpoints

### Order Service

#### Submit Order

Creates a new order and initiates the saga orchestration.

**Endpoint:** `POST /api/orders`

**Request Body:**
```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "totalAmount": 150.00,
  "orderItems": [
    {
      "productId": "a1b2c3d4-e5f6-4789-a012-345678901234",
      "quantity": 2
    },
    {
      "productId": "b2c3d4e5-f6a7-4890-b123-456789012345",
      "quantity": 1
    }
  ]
}
```

**Response:** `202 Accepted`
```json
{
  "orderId": "9fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Note:** The endpoint returns immediately with `202 Accepted` because the order processing is asynchronous. The saga orchestrates the remaining steps in the background.

### Monitoring Saga State

To monitor the saga state, you can query the `OrderStateInstance` table in the database:

```sql
SELECT 
    "CorrelationId",
    "CurrentState",
    "OrderId",
    "CustomerId",
    "TotalAmount",
    "CreatedDate",
    "FaultReason"
FROM "OrderStateInstance"
ORDER BY "CreatedDate" DESC;
```

## Testing the Saga

### Happy Path Scenario

1. **Submit an order** via the Order API:
   ```bash
   curl -X POST "https://localhost:7xxx/api/orders" \
     -H "Content-Type: application/json" \
     -d '{
       "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
       "totalAmount": 150.00,
       "orderItems": [
         {
           "productId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
           "quantity": 2
         }
       ]
     }'
   ```

2. **Monitor the saga progression**:
   - Check RabbitMQ Management UI for message queues
   - Query the `OrderStateInstance` table to see state transitions
   - Check service logs for event processing

3. **Expected flow**:
   - Order submitted → `AwaitingInventory`
   - Inventory reserved → `AwaitingPayment`
   - Payment completed → `Completed`

### Error Scenarios

#### Inventory Reservation Failure

If inventory reservation fails:
- Saga transitions to `Faulted` state
- `FaultReason` is stored in the state instance
- No compensation needed (inventory was never reserved)

#### Payment Failure

If payment processing fails:
- Saga transitions to `Cancelled` state
- Compensation transaction triggers: `ReleaseInventoryCommand` is sent
- Previously reserved inventory is released
- `FaultReason` is stored in the state instance

### Monitoring Tools

1. **RabbitMQ Management UI** (http://localhost:15672):
   - View queues and message flow
   - Monitor message rates
   - Inspect message payloads

2. **Database Queries**:
   - Query `OrderStateInstance` for saga state
   - Query inventory tables for reservation status
   - Check EF Core migration history

3. **Application Logs**:
   - Each service logs saga events and state transitions
   - Look for correlation IDs to trace a specific order through the system

## Key Features

### 1. State Persistence

Saga state is persisted using Entity Framework Core with PostgreSQL:

- **Pessimistic Concurrency**: Row-level locking prevents concurrent state updates
- **Durable State**: Saga state survives service restarts
- **State History**: All state transitions are tracked

### 2. Error Handling

- **Faulted State**: Captures and stores failure reasons
- **Compensation**: Automatic rollback through compensating transactions
- **Retry Logic**: MassTransit provides built-in retry mechanisms
- **Dead Letter Queue**: Failed messages can be moved to DLQ for investigation

### 3. Event-Driven Architecture

- **Asynchronous Communication**: Services communicate via events
- **Loose Coupling**: Services only know about message contracts
- **Scalability**: Services can scale independently
- **Resilience**: Message queues provide buffering and reliability

### 4. Production-Ready Practices

- **Clean Architecture**: Clear separation of concerns
- **Dependency Injection**: Proper IoC container usage
- **CQRS Pattern**: Command/Query separation with MediatR
- **Feature-Based Organization**: Code organized by business features
- **Configuration Management**: Environment-specific settings
- **Logging**: Structured logging throughout the application
- **Swagger Documentation**: API documentation generation

## Architecture Decisions

### Why Orchestration Saga?

This project uses the **Orchestration Saga** pattern (centralized coordinator) rather than **Choreography Saga** (distributed coordination):

**Advantages:**
- **Centralized Control**: Order service has full visibility of the process
- **Easier Debugging**: Single point to monitor saga state
- **Simpler Compensation**: Coordinator can easily trigger compensating transactions
- **Better Error Handling**: Centralized error handling and retry logic

**Trade-offs:**
- Order service becomes a single point of coordination (but not a bottleneck)
- Slightly more coupling (but still loose via message contracts)

### Why MassTransit State Machine?

- **Built-in Saga Support**: Native state machine implementation
- **Persistence**: EF Core integration for state persistence
- **Concurrency Control**: Pessimistic locking prevents race conditions
- **Event Correlation**: Automatic message correlation by ID
- **Production Tested**: Widely used in enterprise applications

### Why PostgreSQL?

- **ACID Compliance**: Strong consistency guarantees
- **JSON Support**: Can store complex state data
- **Performance**: Excellent for read-heavy workloads
- **Open Source**: No licensing costs
- **EF Core Support**: First-class Entity Framework Core support

### Why MediatR?

- **CQRS Implementation**: Natural fit for command/query separation
- **Decoupling**: Handlers are decoupled from controllers
- **Testability**: Easy to unit test use cases
- **Pipeline Behaviors**: Built-in support for cross-cutting concerns

## License

This project is a demonstration/educational project. Please refer to the license file for details.

## Contributing

This is a demo project for educational purposes. Contributions and suggestions are welcome!

## Acknowledgments

- [MassTransit Documentation](https://masstransit.io/)
- [Saga Pattern - Microservices.io](https://microservices.io/patterns/data/saga.html)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

