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
- BCrypt.Net-Next
- openSSL, mTLS

## Use Cases

### AuthService
- **User Registration**: Allows users to register by providing their email and password.
- **User Login**: Authenticates users and provides a JWT token for session management.
- **Token Validation**: Validates the provided JWT tokens for authorized access.

### UserService
- **Create User**: Allows creation of new users with details like name, email, password, and specific role.
- **Get User Details**: Retrieve details of a specific user by their ID.
- **Update User**: Update the information of an existing user.
- **Delete User**: Remove a user from the system.

### OrderService
- **Place Order**: Allows users to place new orders.
- **Get Order Details**: Retrieve details of a specific order by its ID.
- **Update Order**: Update the details of an existing order.
- **Cancel Order**: Cancel an existing order.

### ProductService
- **Add Product**: Allows adding new products to the catalog.
- **Get Product Details**: Retrieve details of a specific product by its ID.
- **Update Product**: Update the information of an existing product.
- **Delete Product**: Remove a product from the catalog.

### NotificationService
- **Send Notification**: Send notifications to users for various events.
- **Get Notification Status**: Check the delivery status of a notification.

## Getting Started

To run the microservices, follow these steps:

1. **Clone the repository**:

    ```bash
    git clone https://github.com/your-username/MicroservicesApp.git
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
