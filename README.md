# MicroservicesApp

This repository contains multiple microservices implemented in C# and Docker. Each microservice is contained in its own directory and has its own Dockerfile for containerization.

## Microservices

1. **AuthService**: Handles authentication and authorization.
2. **NotificationService**: Manages notifications.
3. **OrderService**: Manages orders.
4. **ProductService**: Manages products.
5. **UserService**: Manages user information.

## Docker Compose

A `docker-compose.yml` file is provided to run all microservices together.

## Technologies Used

- C#
- Docker
- Entity Framework Core
- ASP.NET Core

## Getting Started

To run the microservices, use the following commands:

```bash
docker-compose up --build
