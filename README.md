# User Management API

A RESTful API for managing user records with comprehensive validation, logging, error handling, and API key authentication.

## Features

- **CRUD Operations**: Create, Read, Update, and Delete user records
- **Input Validation**: Comprehensive validation with regex patterns and XSS protection
- **Duplicate Prevention**: Prevents duplicate usernames and emails
- **HTTP Logging**: Built-in ASP.NET Core HTTP logging middleware
- **Error Handling**: Global exception handling with standardized JSON responses
- **API Key Authentication**: Secure access with header-based authentication
- **In-Memory Storage**: Simple dictionary-based storage for development

## Technologies

- **Framework**: ASP.NET Core 9.0 (Minimal APIs)
- **Language**: C# with nullable reference types
- **Authentication**: Custom API key middleware
- **Logging**: Built-in ASP.NET Core logging infrastructure

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio Code (recommended) with REST Client extension
- Git (for version control)

### Installation

1. Clone or navigate to the project directory:
   ```powershell
   cd "C:\Users\Camilo Alzate\Desktop\web-api"
   ```

2. Restore dependencies:
   ```powershell
   dotnet restore
   ```

3. Build the project:
   ```powershell
   dotnet build
   ```

### Running the API

1. Start the application:
   ```powershell
   dotnet run
   ```

2. The API will be available at: `http://localhost:5267`

3. You should see HTTP logging output in the console showing incoming requests

### Testing the API

1. Open `web-api.http` in VS Code with REST Client extension installed
2. Click "Send Request" above any test case
3. All requests require the `api-key` header with value: `Kp9mX2nQ7vL4sR8t`

Example using curl:
```powershell
curl -X GET http://localhost:5267/users -H "api-key: Kp9mX2nQ7vL4sR8t"
```

## API Endpoints

All endpoints require the `api-key: Kp9mX2nQ7vL4sR8t` header.

### Get All Users
```
GET /users
```
Returns a list of all users.

**Response**: `200 OK`
```json
[
  {
    "id": "guid",
    "name": "John Doe",
    "username": "johndoe",
    "email": "john.doe@example.com"
  }
]
```

### Get User by ID
```
GET /users/{id}
```
Returns a specific user by their GUID.

**Response**: `200 OK` or `404 Not Found`

### Create User
```
POST /users
Content-Type: application/json

{
  "name": "John Doe",
  "username": "johndoe",
  "email": "john.doe@example.com"
}
```
Creates a new user with auto-generated GUID.

**Response**: `201 Created`
```json
{
  "id": "generated-guid",
  "name": "John Doe",
  "username": "johndoe",
  "email": "john.doe@example.com"
}
```

### Update User
```
PUT /users/{id}
Content-Type: application/json

{
  "name": "John Updated",
  "username": "johnupdated",
  "email": "john.updated@example.com"
}
```
Updates an existing user's information.

**Response**: `200 OK` or `404 Not Found`

### Delete User
```
DELETE /users/{id}
```
Removes a user from the system.

**Response**: `204 No Content` or `404 Not Found`

## Validation Rules

### Name
- **Required**: Yes
- **Length**: 2-50 characters
- **Pattern**: Letters and spaces only (`^[a-zA-Z\s]{2,50}$`)
- **Security**: No HTML/script tags or JavaScript injection

### Username
- **Required**: Yes
- **Length**: 3-20 characters
- **Pattern**: Letters, numbers, hyphens, and underscores (`^[a-zA-Z0-9_-]{3,20}$`)
- **Uniqueness**: Must be unique (case-insensitive)
- **Security**: No HTML/script tags or JavaScript injection

### Email
- **Required**: Yes
- **Pattern**: Valid email format (`^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$`)
- **Uniqueness**: Must be unique (case-insensitive)
- **Security**: No HTML/script tags or JavaScript injection

### Security Features
The API detects and blocks the following malicious patterns:
- `<script>`, `</script>`
- `<iframe>`, `<object>`, `<embed>`
- `javascript:`
- Event handlers: `onerror=`, `onload=`, `onclick=`, `onmouseover=`

## Error Responses

All errors return JSON in the format:
```json
{
  "error": "Error message",
  "statusCode": 400
}
```

### Status Codes
- `200 OK`: Successful GET/PUT request
- `201 Created`: Successful POST request
- `204 No Content`: Successful DELETE request
- `400 Bad Request`: Validation failure or invalid input
- `401 Unauthorized`: Missing or invalid API key
- `404 Not Found`: User not found
- `500 Internal Server Error`: Unexpected server error

## Configuration

### API Key
Located in `appsettings.json`:
```json
{
  "ApiSettings": {
    "ApiKey": "Kp9mX2nQ7vL4sR8t"
  }
}
```

**For production**: Move this to environment variables or user secrets.

### Logging Levels
Configure in `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware": "Information"
    }
  }
}
```

## Project Structure

```
web-api/
├── Models/
│   └── User.cs                    # User data model
├── Helpers/
│   └── ValidationHelper.cs        # Validation logic with regex patterns
├── Middleware/
│   ├── ApiKeyAuthenticationMiddleware.cs  # API key validation
│   └── RequestLoggingMiddleware.cs        # (Legacy - not used)
├── Program.cs                     # Application entry point and endpoint definitions
├── appsettings.json              # Configuration including API key
└── web-api.http                  # REST Client test file

```

## Middleware Pipeline

The middleware executes in the following order:

1. **Exception Handler** (Production) / **Developer Exception Page** (Development)
2. **HTTP Logging Middleware** - Logs all HTTP requests/responses
3. **API Key Authentication Middleware** - Validates API key header
4. **HTTPS Redirection** - (Commented out for local development)
5. **Endpoints** - User management routes

## Development Notes

- **In-Memory Storage**: Data is lost when the application stops. Not suitable for production.
- **No Thread Safety**: Current implementation is not thread-safe for concurrent requests.
- **HTTPS**: Disabled for local development without SSL certificates.
- **API Key Security**: Use environment variables or Azure Key Vault in production.

## Future Enhancements

- Database integration (Entity Framework Core with SQL Server/PostgreSQL)
- JWT-based authentication instead of API keys
- Role-based authorization (Admin, User roles)
- Pagination for GET all users
- Search and filtering capabilities
- Password hashing for user credentials
- Email verification workflow
- Rate limiting
- API versioning
- Swagger/OpenAPI documentation UI

## Troubleshooting

### API returns 401 Unauthorized
- Ensure you're sending the `api-key` header with value `Kp9mX2nQ7vL4sR8t`
- Check that the header name is exactly `api-key` (case-sensitive)

### Port already in use
- Change the port in `Properties/launchSettings.json`
- Or kill the process using the port: `Get-Process -Id (Get-NetTCPConnection -LocalPort 5267).OwningProcess | Stop-Process`

### Validation errors
- Check that all required fields are provided
- Verify field lengths and patterns match the validation rules
- Ensure no HTML/script tags in input

## License

This project is for educational purposes as part of a .NET API backend development certification course.

## Author

Created as part of a TechHive Solutions training exercise for building User Management APIs.
