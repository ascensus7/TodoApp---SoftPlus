# TodoApp

TodoApp is a full-stack task management application built as a technical
assessment. It provides secure, user-specific task and category management
through an ASP.NET Core API and an Angular client.

## Features

- User registration and sign-in
- JWT-based authentication and protected routes
- User-specific tasks and categories
- Create, view, update, complete, and delete tasks
- Create, rename, and delete categories
- Search tasks by title or description
- Filter tasks by category and completion status
- Server-side pagination
- Request validation and centralized API error handling
- Responsive interface built with Bootstrap

## Technology Stack

| Layer | Technology |
| --- | --- |
| Backend | ASP.NET Core Web API, .NET 10 |
| Data access | Entity Framework Core 10 |
| Authentication | ASP.NET Core Identity, JWT Bearer |
| Database | PostgreSQL |
| Frontend | Angular 22, TypeScript, RxJS |
| UI | Bootstrap 5, Bootstrap Icons |
| API specification | OpenAPI |

## Project Structure

```text
TodoApp/
├── TodoApp.Api/          API controllers, middleware, and configuration
├── TodoApp.Services/     Application services and business logic
├── TodoApp.DataAccess/   EF Core context, Identity, mappings, and migrations
├── TodoApp.Domain/       Domain entities
├── TodoApp.Interfaces/   DTOs, shared response models, and service contracts
└── todo-app-client/      Angular client application
```

The backend follows a layered structure:

```text
Angular Client → API Controllers → Application Services → EF Core → PostgreSQL
```

Controllers handle HTTP concerns, services contain application logic, and the
data access layer is responsible for persistence. Tasks and categories are
always queried using the authenticated user's identifier.

## Prerequisites

Install the following software:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Node.js](https://nodejs.org/) with npm
- Entity Framework Core CLI

Install the EF Core CLI if it is not already available:

```bash
dotnet tool install --global dotnet-ef
```

You can verify the required tools with:

```bash
dotnet --version
dotnet ef --version
node --version
npm --version
```

## Running the Application

The backend and frontend run as separate applications. Commands in this
section are executed from the repository root.

### 1. Start PostgreSQL

Make sure that a PostgreSQL server is running and that you have a user with
permission to create and update the application database.

The examples below assume:

```text
Host: localhost
Port: 5432
Database: todo_app
Username: postgres
```

### 2. Configure Local Secrets

The repository does not contain database credentials or a JWT signing key.
Configure them with .NET User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=todo_app;Username=postgres;Password=YOUR_PASSWORD" --project TodoApp.Api
dotnet user-secrets set "Jwt:Key" "replace-this-with-a-secure-key-at-least-32-characters-long" --project TodoApp.Api
```

Replace `YOUR_PASSWORD` and the example signing key with your own values. If
your PostgreSQL server uses different connection settings, update the
connection string accordingly.

To confirm that the values were saved:

```bash
dotnet user-secrets list --project TodoApp.Api
```

> User Secrets are intended for local development only. Production secrets
> should be supplied through environment variables or a dedicated secret
> manager.

### 3. Restore the Backend and Apply Migrations

```bash
dotnet restore
dotnet ef database update --project TodoApp.DataAccess --startup-project TodoApp.Api
```

The migration command creates or updates the schema using the migrations in
`TodoApp.DataAccess/Migrations`.

### 4. Run the API

```bash
dotnet run --project TodoApp.Api --launch-profile https
```

The API starts at:

```text
HTTPS: https://localhost:7164
HTTP:  http://localhost:5021
```

The Angular client is configured to use `https://localhost:7164/api`.

If your browser does not trust the ASP.NET Core development certificate, run:

```bash
dotnet dev-certs https --trust
```

Then restart the API and your browser.

### 5. Install and Run the Angular Client

Open a second terminal:

```bash
cd todo-app-client
npm ci
npm start
```

Open [http://localhost:4200](http://localhost:4200).

No demo account is seeded. Create an account on the registration page, sign
in, and then create categories and tasks.

## API Overview

The authentication endpoints are public. All category and task endpoints
require an access token in the following header:

```http
Authorization: Bearer <token>
```

### Authentication

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/auth/register` | Register a user and return an access token |
| `POST` | `/api/auth/login` | Authenticate a user and return an access token |

The minimum password length is six characters and the password must contain at
least one digit and one lowercase letter.

### Categories

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/categories` | Get the current user's categories |
| `GET` | `/api/categories/{id}` | Get a category by ID |
| `POST` | `/api/categories` | Create a category |
| `PUT` | `/api/categories/{id}` | Rename a category |
| `DELETE` | `/api/categories/{id}` | Delete a category |

### Tasks

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/todo-items` | Get a paginated task list |
| `GET` | `/api/todo-items/{id}` | Get a task by ID |
| `POST` | `/api/todo-items` | Create a task |
| `PUT` | `/api/todo-items/{id}` | Update a task |
| `DELETE` | `/api/todo-items/{id}` | Delete a task |

The task list supports the following query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `pageNumber` | integer | Page number, starting at `1` |
| `pageSize` | integer | Items per page, from `1` to `100` |
| `search` | string | Search term, up to 100 characters |
| `categoryId` | integer | Category identifier |
| `isCompleted` | boolean | Completion status |

Example:

```http
GET /api/todo-items?pageNumber=1&pageSize=10&search=report&categoryId=1&isCompleted=false
```

## API Documentation and Manual Testing

When the API runs in the Development environment, its OpenAPI document is
available at:

[https://localhost:7164/openapi/v1.json](https://localhost:7164/openapi/v1.json)

Swagger UI is not included. Ready-to-run requests for registration,
authentication, categories, and tasks are provided in
`TodoApp.Api/TodoApp.Api.http`.

To use protected requests:

1. Run the register or login request.
2. Copy the returned token.
3. Set the `token` variable at the top of `TodoApp.Api.http`.
4. Run the required category or task request.

## Configuration

Backend settings are defined in `TodoApp.Api/appsettings.json` and can be
overridden through User Secrets or environment variables.

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `Jwt:Key` | Secret used to sign access tokens |
| `Jwt:Issuer` | Expected token issuer |
| `Jwt:Audience` | Expected token audience |
| `Jwt:ExpirationMinutes` | Access-token lifetime |
| `AllowedOrigins` | Origins allowed by the CORS policy |

The client API address is configured in:

```text
todo-app-client/src/environments/environment.ts
```

If the client or API address changes, update both the frontend `apiUrl` and the
backend `AllowedOrigins` setting.

## Build

Build the complete backend solution:

```bash
dotnet build
```

Build the Angular client:

```bash
cd todo-app-client
npm ci
npm run build
```

The optimized frontend output is written to `todo-app-client/dist`.

Automated test projects are not included in the current repository.

## Troubleshooting

### The API stops during startup

Make sure both required secrets are configured:

```bash
dotnet user-secrets list --project TodoApp.Api
```

The output must contain `ConnectionStrings:DefaultConnection` and `Jwt:Key`.

### Database connection fails

Confirm that PostgreSQL is running and that the connection string contains the
correct host, port, database, username, and password. Then run the migration
command again.

### The browser reports a certificate error

Trust the local development certificate:

```bash
dotnet dev-certs https --trust
```

Restart the API and browser afterwards.

### The browser reports a CORS error

The default CORS policy allows `http://localhost:4200`. If Angular is running
at another address, add that origin to `AllowedOrigins`.

### The API returns `401 Unauthorized`

Sign in again and verify that requests include a valid Bearer token. Tokens
expire after the interval configured in `Jwt:ExpirationMinutes`.

### The frontend cannot reach the API

Verify that:

- the API is running at `https://localhost:7164`;
- the development certificate is trusted;
- `apiUrl` in the Angular environment file matches the API address.

## License

This project is distributed under the terms of the [MIT License](LICENSE.txt).
