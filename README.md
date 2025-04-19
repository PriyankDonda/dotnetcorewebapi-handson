# Handson API

A .NET Core Web API project with JWT authentication and security features.

## Features

- JWT Authentication
- User Registration and Login
- Global Exception Handling
- Security Headers
- Enhanced JWT Configuration

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (or your preferred database)

### Installation

1. Clone the repository
```bash
git clone https://github.com/yourusername/Handson.git
```

2. Navigate to the project directory
```bash
cd Handson
```

3. Restore dependencies
```bash
dotnet restore
```

4. Run the application
```bash
dotnet run
```

## API Endpoints

### Authentication

- POST `/api/auth/register` - Register a new user
- POST `/api/auth/login` - Login user and get JWT token

## Security Features

- JWT Token Authentication
- Security Headers
- Global Exception Handling
- HTTPS Enforcement
- Token Validation
- XSS Protection
- CSRF Protection

## License

This project is licensed under the MIT License. 