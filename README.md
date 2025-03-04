# Trading Microservice

A microservice implementation for a trading system that allows clients to execute and retrieve trades. Built as part of transitioning a bank's monolithic architecture to microservices.

## Features

- Execute trades via REST API
- Retrieve trades with filtering capabilities
- Persist trade information in a database
- Publish trade events to Kafka for event-driven architecture
- Console application to monitor trades in real-time

## Architecture

This solution follows a clean architecture approach with the following components:

```
TradingMicroservice/
├── src/
│   ├── TradingMicroservice.API/           # REST API endpoints
│   ├── TradingMicroservice.Core/          # Domain models and interfaces
│   ├── TradingMicroservice.Infrastructure/ # Repository and service implementations
│   └── TradingMicroservice.Console/       # Trade monitoring console app
├── tests/
│   ├── TradingMicroservice.UnitTests/     # Unit tests
│   └── TradingMicroservice.IntegrationTests/ # Integration tests
└── docker-compose.yml                     # Docker container configuration
```

## Technology Stack

- **C# (.NET Core)** - Primary programming language and framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database for storing trade information
- **Kafka** - Message broker for trade events
- **RESTful API** - HTTP interface for executing and retrieving trades
- **xUnit & Moq** - Testing frameworks
- **Docker** - Containerization for services

## Getting Started

### Prerequisites

- .NET Core SDK 3.1 or higher
- Docker Desktop

### Setup

1. Clone the repository

2. Start the infrastructure services using Docker Compose:
   ```
   docker-compose up -d
   ```

3. Create the Kafka topic:
   ```
   docker exec -it work-kafka-1 kafka-topics --bootstrap-server kafka:9092 --create --topic trades --partitions 1 --replication-factor 1
   ```

4. Apply database migrations:
   ```
   cd TradingMicroservice.Infrastructure
   dotnet ef database update --startup-project ../TradingMicroservice.API
   ```

5. Start the API:
   ```
   cd TradingMicroservice.API
   dotnet run
   ```

6. Start the Console monitoring application:
   ```
   cd TradingMicroservice.Console
   dotnet run
   ```

### API Endpoints

#### Execute Trade
```
POST /api/trades
```
Request body:
```json
{
    "symbol": "AAPL",
    "quantity": 100,
    "price": 150.00,
    "clientId": "client123",
    "type": 0  // 0 for Buy, 1 for Sell
}
```

#### Get All Trades
```
GET /api/trades
```
Optional query parameters:
- `clientId` - Filter by client ID
- `from` - Filter by execution time (start date)
- `to` - Filter by execution time (end date)

#### Get Trade by ID
```
GET /api/trades/{id}
```

## Testing

Run the unit tests:
```
cd TradingMicroservice.UnitTests
dotnet test
```

## Monitoring

### Kafka Topics
You can monitor Kafka topics using Kafdrop at http://localhost:9000 or using the command line:
```
docker exec -it work-kafka-1 kafka-console-consumer --bootstrap-server localhost:9092 --topic trades --from-beginning
```

### Database
Connect to SQL Server using:
- Server: localhost,1433
- Username: sa
- Password: YourStrong!Pwd123
- Database: TradingDb

## Design Decisions

### Event-Driven Architecture
Trade execution events are published to Kafka, allowing other services to react to trades without tight coupling.

### Repository Pattern
Abstracts data access logic and makes the system testable and maintainable.

### CQRS-like Approach
Separate endpoints for commands (executing trades) and queries (retrieving trades).

### Resilience
- Retry capabilities for database operations
- Error handling and proper status codes
- Message persistence in Kafka

## Future Enhancements

- Authentication and authorization
- Rate limiting
- Circuit breaker pattern
- Distributed tracing
- Metrics and monitoring dashboards
- HTTPS encryption
- Input validation
