# Handson API

A .NET Core Web API project with JWT authentication, PostgreSQL, Dapper, and security features.

## Features

- JWT Authentication
- User Registration and Login
- PostgreSQL with Dapper ORM
- Redis Caching
- Global Exception Handling
- Security Headers
- Enhanced JWT Configuration
- Rate Limiting
- Serilog Logging

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL 15 or later
- Redis Server (for caching)

### Installation

1. Clone the repository
```bash
git clone https://github.com/PriyankDonda/dotnetcorewebapi-handson.git
```

2. Navigate to the project directory
```bash
cd Handson
```

3. Configure the application
   - Copy `appsettings.Example.json` to `appsettings.json`
   - Update the configuration values in `appsettings.json` with your settings:
     - PostgreSQL connection string
     - Redis connection string
     - JWT secret key and settings
     - Rate limiting configuration
     - Other environment-specific configurations

4. Restore dependencies
```bash
dotnet restore
```

5. Run the application
```bash
dotnet run
```

## Configuration

The application requires the following configuration in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your_host;Port=5432;Database=your_database;Username=your_username;Password=your_password",
    "Redis": "your_redis_host:6379"
  },
  "Jwt": {
    "Key": "your_jwt_secret_key",
    "Issuer": "your_issuer",
    "Audience": "your_audience"
  },
  "RateLimiting": {
    "RequestLimit": 100,
    "TimeWindowSeconds": 60
  }
}
```

**Important**: Never commit your actual `appsettings.json` file to source control as it may contain sensitive information.

## API Endpoints

### Authentication

- POST `/api/auth/register` - Register a new user
- POST `/api/auth/login` - Login user and get JWT token

## Technology Stack

- **Framework**: .NET 8.0
- **Database**: PostgreSQL
- **ORM**: Dapper
- **Caching**: Redis
- **Authentication**: JWT
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI

## Security Features

- JWT Token Authentication
- Security Headers
- Global Exception Handling
- HTTPS Enforcement
- Token Validation
- XSS Protection
- CSRF Protection
- Rate Limiting
- Secure Configuration Management

## License

This project is licensed under the MIT License. 