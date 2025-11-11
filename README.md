# About
A simple application built to demonstrate microservices architecture with ASP.NET Core.
It is based on dotnetcore-microservices-poc (https://github.com/asc-lab/dotnetcore-microservices-poc) with additional features.

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
You must install Docker & Docker Compose before.
Scripts have been divided into two parts:

- infra.yml runs the necessary infrastructure.
- app.yml is used to run the application.
You can use scripts to build/run/stop/down all containers.


```bash
cd scripts/
./seed.sh
./run.sh
```

Access the API gateway at `http://localhost:5432`
If the app contains problems, then re run the above run.sh script again.

If the app does not work correctly, you may either:

- Rerun the run.sh script
- Run each file of infra.yml and app.yml using the following code

```bash
docker compose -f infra.yml up
docker compose -f app.yml up
```
