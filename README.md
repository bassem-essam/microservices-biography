# About
A simple application built to demonstrate microservices architecture with ASP.NET Core.

It is based on the awesome blog post series ["How to build .NET Core microservices"](https://www.altkomsoftware.com/blog/building-microservices-net-core-part-1-plan/) with some additional features.

# Architecture
This is a block diagram of the whole application architecture where straight lines mean synchronous messaging and dashed lines mean asynchronous messaging.

![biography_architecture_final](https://github.com/user-attachments/assets/6f78768b-cc21-4b1c-abe6-a983d4c7a721)

# Features
- Authentication with JWT
- CQRS with MediatR
- ORM with Entity Framework Core
- Caching with Redis
- API Gateway with Ocelot
- Database management with SQL Server and MongoDB
- Service discovery with Eureka
- Synchronous communication via HTTP and gRPC
- Asynchronous messaging with RabbitMQ
- Image generation with SkiaSharp
- Containerization with Docker and Docker Compose

# Getting Started

**Prerequisites:** Docker & Docker Compose

Scripts are divided into two parts:
- `infra.yml` - Runs necessary infrastructure (databases, message broker, etc.)
- `app.yml` - Runs the application services

### Quick Start
```bash
cd scripts/
./seed.sh
./run.sh
```

Access the API gateway at `http://localhost:5432`

### Troubleshooting

If the app has issues:
1. Rerun `./run.sh`
2. Or run infrastructure and application separately:
```bash
docker compose -f infra.yml up
docker compose -f app.yml up
```
