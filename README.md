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
- RabbitMQ (for inter-service communication)
- JWT (JSON Web Tokens for authentication)
- Microsoft.Extensions.Http (for HTTP client factory and dependency injection)

## Getting Started

To run the microservices, follow these steps:

1. **Clone the repository**:

    ```bash
    git clone https://github.com/xwonderwallx/MicroservicesApp.git
    cd MicroservicesApp
    ```

2. **Set up Docker and RabbitMQ**:

    Ensure you have Docker installed and running on your machine. Docker Compose will also start RabbitMQ for you.

3. **Build and run the microservices**:

    Use Docker Compose to build and run all the services together:

    ```bash
    docker-compose up --build
    ```

4. **Access the services**:

    Each service will be available at the following URLs (adjust ports as necessary):

    - AuthService: http://localhost:5107
    - NotificationService: http://localhost:5004
    - OrderService: http://localhost:5003
    - ProductService: http://localhost:5002
    - UserService: http://localhost:5001

## Environment Variables

Make sure to set the necessary environment variables for each service. You can configure these in the Docker Compose file or in the individual service settings.

## Running Tests

To run the unit tests for each microservice, navigate to the test project directory and use the following command:

```bash
dotnet test
