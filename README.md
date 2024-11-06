# Blog API

This project is a RESTful Web API for managing blog posts. It includes full CRUD (Create, Read, Update, Delete) operations for blog entries and follows best practices, including proper error handling, validation, logging, and documentation. This API is built with .NET Core and uses SQLite as its data store.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Setup](#project-setup)
- [Project Architecture](#project-architecture)
- [Design Decisions](#design-decisions)
- [Error Handling and Logging](#error-handling-and-logging)
- [Validation](#validation)
- [Additional Features](#additional-features)
- [Future Enhancements](#future-enhancements)

---

## Features

- **CRUD Operations**: Create, retrieve, update, and delete blog posts.
- **Input Validation**: Ensures data integrity and proper formatting.
- **Logging**: Uses Serilog for detailed logging across the application.
- **Error Handling**: Comprehensive error handling with informative error messages.
- **Docker Support**: Can be containerized using Docker for easier deployment.

## Technologies Used

- **.NET Core 6**: The framework for building the API.
- **SQLite**: Lightweight database for data persistence.
- **Entity Framework Core**: ORM for database management.
- **Serilog**: Library for logging, configured to write logs to both console and file.
- **Swagger**: API documentation and testing tool (enabled in development mode).

## Project Setup

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQLite](https://www.sqlite.org/download.html) (installed or configured within Docker)
- Docker (optional, if containerizing the application)

### Installation

1. **Clone the repository**:

    ```bash
    git clone https://github.com/yourusername/BlogApiProject.git
    cd BlogApiProject
    ```

2. **Restore dependencies**:

    ```bash
    dotnet restore
    ```

3. **Database Configuration**:

   The project is configured to use SQLite with a connection string in `appsettings.json`. This file should look like this:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Data Source=blog.db"
      },
      ...
    }
    ```

4. **Run Migrations**:

    Apply database migrations to set up the required tables:

    ```bash
    dotnet ef database update
    ```

5. **Run the Application**:

    Start the API:

    ```bash
    dotnet run
    ```

6. **Swagger Documentation**:

    Open `https://localhost:5001/swagger` in your browser to view and test API endpoints.

### Docker Setup (Optional)

1. **Build the Docker image**:

    ```bash
    docker build -t blogapi .
    ```

2. **Run the Docker container**:

    ```bash
    docker run -d -p 5000:80 blogapi
    ```

## Project Architecture

This project is organized following a layered architecture:

- **Controllers**: Define API endpoints and handle HTTP requests. Controllers validate inputs and return appropriate responses.
- **Services**: Handle business logic and interact with repositories.
- **Repositories**: Manage database operations, including CRUD operations for blog posts.
- **Models**: Define data structures, including validation rules using data annotations.
- **Logging**: Implemented with Serilog, logging messages for important actions, warnings, and errors.

## Design Decisions

1. **Layered Architecture**: Separating concerns between Controllers, Services, and Repositories ensures modularity and improves testability.
2. **SQLite**: Chosen as a lightweight database option, ideal for development and testing.
3. **Dependency Injection**: All services and repositories are registered with dependency injection, following the Inversion of Control (IoC) principle.
4. **Logging with Serilog**: Enables detailed and structured logging, crucial for troubleshooting and monitoring.

## Error Handling and Logging

- **Global Error Handling**: Errors are caught and handled in the Controller layer, with descriptive error responses and status codes.
- **Input Validation**: Validation rules are enforced at the model level using data annotations, such as `[Required]`, `[MaxLength]`, and `[RegularExpression]`.
- **Logging**: Serilog logs messages to both the console and a file. The log levels used include:
  - **Information**: For successful operations.
  - **Warning**: For invalid operations, such as when a resource is not found.
  - **Error**: For unexpected issues, logged with stack trace information.

## Validation

The following validations are implemented:

- **Title**: Required and limited to 100 characters.
- **Content**: Required.
- **Author**: Required, restricted to alphabetic characters only (validated using a regular expression).
- **Date**: Auto-generated on creation.

These validation rules are enforced through model annotations and checked within the controller actions to ensure data integrity before proceeding to the service layer.

## Additional Features

1. **Swagger Integration**: Swagger UI is enabled in development mode, providing interactive API documentation.
2. **Containerization**: A Dockerfile is provided for easy deployment in a containerized environment.

## Future Enhancements

- **Authentication and Authorization**: Implement authentication to restrict access to certain endpoints.
- **Additional Data Models**: Expand the data model to include related entities like `Categories` or `Comments`.
- **Enhanced Validation**: Use FluentValidation for more complex validation rules.
- **Unit and Integration Testing**: Add comprehensive tests for each component.

---

This README provides detailed information on the project’s setup, architecture, and design, ensuring it’s easy for others to understand, set up, and extend the project.
