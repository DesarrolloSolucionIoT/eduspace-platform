# EduSpace Platform

Educational management platform built with .NET 8, Clean Architecture, and Domain-Driven Design.

## Documentation

- **[PROJECT.md](PROJECT.md)** — authoritative backend reference (routes, auth flows, DB schema, env vars). When this file and the README disagree, PROJECT.md wins.
- **[../CLAUDE.md](../CLAUDE.md)** — workspace context for AI-assisted development sessions.

---

## Table of Contents

- [About](#about)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [API Documentation](#api-documentation)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Development](#development)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License](#license)

---

## About

EduSpace Platform is an educational management system for academic institutions. Built as a modular monolith, it provides bounded-context isolation with a single deployment unit. The platform covers user authentication, space reservations, meeting scheduling, and maintenance reporting via a RESTful API.

---

## Architecture

EduSpace follows Clean Architecture (Hexagonal / Screaming) with DDD patterns, organized into bounded contexts:

```
┌─────────────────────────────────────────────────────────┐
│                    API Layer (REST)                      │
│              Controllers, Resources, DTOs                │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│                  Application Layer                       │
│         CQRS Commands/Queries, Use Cases                 │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│                   Domain Layer                           │
│     Aggregates, Entities, Value Objects, Interfaces     │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│               Infrastructure Layer                       │
│       EF Core, Repositories, External Services          │
└─────────────────────────────────────────────────────────┘
```

### Bounded Contexts

| Context | Responsibility |
|---------|---------------|
| **IAM** | Authentication, authorization, account management |
| **Profiles** | Teacher and administrator profile management |
| **SpacesAndResourceManagement** | Classrooms, resources, shared areas |
| **ReservationScheduling** | Meeting planning and teacher participation |
| **BreakdownManagement** | Maintenance and incident reporting |

### Key Design Patterns

- **CQRS** — Command Query Responsibility Segregation
- **Repository Pattern** with Unit of Work
- **Anti-Corruption Layer (ACL)** for inter-context communication
- **Value Objects** for domain primitives
- **Aggregate Roots** for consistency boundaries

---

## Tech Stack

- **.NET 8.0** / ASP.NET Core — web API framework
- **Entity Framework Core 8** — ORM (`MySql.EntityFrameworkCore` Oracle provider)
- **MySQL 8.0** — relational database
- **JWT (HMAC-SHA256)** via `JsonWebTokenHandler` — stateless authentication
- **BCrypt.Net-Next** — password hashing
- **SendGrid** (`SendGrid` 9.29.3) — transactional email (activation links)
- **Swagger/OpenAPI** via Swashbuckle — interactive docs (development only)
- **DotNetEnv** — `.env` file loading at startup
- **Humanizer** — snake_case table/column naming
- **EntityFrameworkCore.CreatedUpdatedDate** — automatic `created_at`/`updated_at` timestamps
- **xUnit + FluentAssertions + NSubstitute** — test framework

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL 8.0](https://dev.mysql.com/downloads/) or Docker (to run MySQL via docker-compose)
- A [SendGrid](https://sendgrid.com) account with an API key — **required in every environment**; the app throws at startup if `SENDGRID_API_KEY` is absent
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional, for running MySQL)
- [Git](https://git-scm.com/downloads)

**Recommended IDE:** Visual Studio 2022, JetBrains Rider, or VS Code with the C# extension.

---

## Quick Start

### Option 1: Local development (API on host, MySQL in Docker)

#### 1. Clone the repository

```bash
git clone https://github.com/DesarrolloSolucionIoT/eduspace-platform.git
cd eduspace-platform
```

#### 2. Configure environment variables

```bash
cp .env.example .env
```

Edit `.env`:

```env
# Database (docker-compose uses these)
MYSQL_ROOT_PASSWORD=your_root_password
MYSQL_DATABASE=eduspacedb
MYSQL_USER=eduspace
MYSQL_PASSWORD=your_mysql_password
MYSQL_PORT=3308

# Connection string for the API
ConnectionStrings__DefaultConnection=server=localhost;port=3308;user=eduspace;password=your_mysql_password;database=eduspacedb;AllowPublicKeyRetrieval=true;SslMode=none

# JWT (secret must be at least 32 characters)
TokenSettings__Secret=your-secret-key-minimum-32-characters

# Email — SendGrid (mandatory; startup fails if absent)
SENDGRID_API_KEY=SG.xxxxxxxxxxxxxxxxxxxx
SENDGRID_FROM_EMAIL=noreply@yourdomain.com
SENDGRID_FROM_NAME=EduSpace

# Frontend base URL — used in activation link emails
FRONTEND_BASE_URL=http://localhost:5173
```

#### 3. Start MySQL

```bash
# docker-compose.yml starts MySQL only; the API runs on the host
docker-compose up -d
```

#### 4. Restore and run

```bash
dotnet restore
dotnet run --project FULLSTACKFURY.EduSpace.API/FULLSTACKFURY.EduSpace.API.csproj
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger` (development only)

EF migrations run automatically on startup via `context.Database.Migrate()`.

### Option 2: Docker (full stack)

Build and run the API image manually alongside the MySQL container:

```bash
docker build -t eduspace-platform:latest .
docker-compose up -d          # start MySQL
docker run -d \
  -p 8080:8080 \
  --env-file .env \
  --name eduspace-api \
  eduspace-platform:latest
```

- **API**: `http://localhost:8080`
- **Swagger**: not available in production mode; set `ASPNETCORE_ENVIRONMENT=Development` to enable

---

## API Documentation

For the full route reference with request/response shapes, see [PROJECT.md](PROJECT.md).

### Interactive documentation

Swagger UI is available in development mode at `https://localhost:5001/swagger`. It is disabled in production.

### Authentication flow

#### 1. Create an administrator account (anonymous)

```http
POST /api/v1/administrator-profiles
Content-Type: application/json

{
  "username": "admin@example.com",
  "password": "SecurePass123!",
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "admin@example.com",
  "dni": "12345678",
  "address": "123 Main St",
  "phone": "912345678"
}
```

This creates an `Account` (IsActive=false) and an `AdminProfile`, then sends an activation email via SendGrid. The email contains a link: `{FRONTEND_BASE_URL}/activate?token=<uuid-token>`.

Teacher accounts are created by an authenticated administrator via `POST /api/v1/teachers-profiles`; they are activated immediately without an email.

#### 2. Activate the account

```http
POST /api/v1/authentication/activate
Content-Type: application/json

{
  "token": "<raw-token-from-email-link>"
}
```

Returns `204 No Content` on success. The token is single-use and valid for 24 hours.

#### 3. Sign in

```http
POST /api/v1/authentication/sign-in
Content-Type: application/json

{
  "username": "admin@example.com",
  "password": "SecurePass123!"
}
```

Returns `200` with the full authentication bundle, or `403` if the account has not been activated yet.

```json
{
  "id": 1,
  "profileId": 5,
  "username": "admin@example.com",
  "role": "RoleAdmin",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "f3a9b6c2...",
  "accessTokenExpiresIn": 3600
}
```

#### 4. Authenticate subsequent requests

```http
Authorization: Bearer <accessToken>
```

#### 5. Refresh / Logout

```http
POST /api/v1/authentication/refresh
Content-Type: application/json
{ "refreshToken": "f3a9b6c2..." }
```

```http
POST /api/v1/authentication/logout
Content-Type: application/json
{ "refreshToken": "f3a9b6c2..." }
```

### Core endpoints

#### Authentication

```http
POST /api/v1/authentication/sign-up        # (legacy alias — prefer profile creation endpoints)
POST /api/v1/authentication/sign-in
POST /api/v1/authentication/activate       # { "token": "<raw-token>" }
POST /api/v1/authentication/refresh
POST /api/v1/authentication/logout
```

#### Teacher Profiles

```http
POST   /api/v1/teachers-profiles
GET    /api/v1/teachers-profiles
GET    /api/v1/teachers-profiles/{id}
PUT    /api/v1/teachers-profiles/{id}
DELETE /api/v1/teachers-profiles/{id}
```

#### Administrator Profiles

```http
POST   /api/v1/administrator-profiles       # [AllowAnonymous]
GET    /api/v1/administrator-profiles
GET    /api/v1/administrator-profiles/{id}
PUT    /api/v1/administrator-profiles/{id}
DELETE /api/v1/administrator-profiles/{id}
```

#### Classrooms

```http
GET    /api/v1/classrooms
GET    /api/v1/classrooms/{id}
GET    /api/v1/classrooms/teachers/{teacherId}
POST   /api/v1/classrooms/teachers/{teacherId}
PUT    /api/v1/classrooms/{id}
DELETE /api/v1/classrooms/{id}
```

#### Resources (scoped to a classroom)

```http
POST   /api/v1/classrooms/{classroomId}/resources
GET    /api/v1/classrooms/{classroomId}/resources
GET    /api/v1/classrooms/{classroomId}/resources/{resourceId}
PUT    /api/v1/classrooms/{classroomId}/resources/{resourceId}
DELETE /api/v1/classrooms/{classroomId}/resources/{resourceId}
```

#### Shared Areas

```http
GET    /api/v1/shared-area
GET    /api/v1/shared-area/{id}
POST   /api/v1/shared-area
PUT    /api/v1/shared-area/{id}
DELETE /api/v1/shared-area/{id}
```

#### Meetings

```http
GET    /api/v1/meetings
GET    /api/v1/meetings/{id}
GET    /api/v1/administrators/{adminId}/meetings
GET    /api/v1/teachers/{teacherId}/meetings
POST   /api/v1/administrators/{adminId}/classrooms/{classroomId}/meetings
PUT    /api/v1/meetings/{id}
DELETE /api/v1/meetings/{id}
```

#### Meeting Participants

```http
POST   /api/v1/meetings/{meetingId}/teachers/{teacherId}
DELETE /api/v1/meetings/{meetingId}/teachers/{teacherId}
```

#### Reports (Breakdowns)

```http
GET    /api/v1/reports
GET    /api/v1/reports/{id}
GET    /api/v1/reports/resources/{resourceId}
POST   /api/v1/reports
PUT    /api/v1/reports/{id}
DELETE /api/v1/reports/{id}
```

---

## Project Structure

```
eduspace-platform/
├── FULLSTACKFURY.EduSpace.API/
│   ├── IAM/                             # Identity & Access Management
│   │   ├── Domain/Model/Aggregates/     # Account, RefreshToken, ActivationToken
│   │   ├── Application/Internal/        # AccountCommandService, RefreshTokenService
│   │   ├── Infrastructure/
│   │   │   ├── Hashing/BCrypt/          # BCrypt wrapper
│   │   │   ├── Tokens/JWT/              # TokenService (JsonWebTokenHandler)
│   │   │   ├── Services/                # EmailService (SendGrid)
│   │   │   └── Pipeline/Middleware/     # GlobalExceptionHandler
│   │   └── Interfaces/
│   │       ├── REST/                    # AuthenticationController
│   │       └── ACL/                    # IamContextFacade (outbound to Profiles)
│   │
│   ├── Profiles/                        # Admin and Teacher profile management
│   │   ├── Domain/Model/Aggregates/     # AdminProfile, TeacherProfile, Profile (base)
│   │   ├── Application/Internal/        # Command/query services + ACL outbound
│   │   ├── Infrastructure/Persistence/  # EF Core repositories
│   │   └── Interfaces/
│   │       ├── REST/                    # AdministratorProfilesController, TeachersProfilesController
│   │       └── ACL/                    # ProfilesContextFacade (inbound)
│   │
│   ├── SpacesAndResourceManagement/     # Classrooms, Resources, Shared Areas
│   ├── ReservationScheduling/           # Meetings and MeetingParticipants
│   ├── BreakdownManagement/             # Breakdown reports
│   │   └── Interfaces/REST/            # ReportController
│   │
│   ├── Shared/                          # Cross-cutting infrastructure
│   │   ├── Domain/Repositories/         # IBaseRepository
│   │   └── Infrastructure/Persistence/EFC/
│   │       ├── Configuration/           # AppDbContext, ModelBuilderExtensions
│   │       └── Repositories/           # BaseRepository, UnitOfWork
│   │
│   ├── Migrations/                      # EF Core migration files
│   ├── Program.cs
│   └── appsettings.json
│
├── FULLSTACKFURY.EduSpace.API.Tests/    # xUnit test project
├── Dockerfile
├── docker-compose.yml                   # MySQL only — API runs on host
├── deploy.sh                            # Azure Container Apps deploy script
└── .env.example
```

---

## Configuration

### Database

EF Core migrations run automatically on startup. To add a new migration after changing the model:

```bash
dotnet ef migrations add YourMigrationName --project FULLSTACKFURY.EduSpace.API
```

Connection string format:
```
server=localhost;port=3308;user=eduspace;password=your_password;database=eduspacedb;AllowPublicKeyRetrieval=true;SslMode=none
```

Set via `ConnectionStrings__DefaultConnection` in `.env` or environment.

### JWT token settings

Defaults in `appsettings.json` (override via environment variables):

```json
{
  "TokenSettings": {
    "Secret": "",
    "Issuer": "EduSpace",
    "Audience": "EduSpaceUsers",
    "AccessTokenLifetimeMinutes": 60,
    "RefreshTokenLifetimeDays": 14
  }
}
```

`Secret` must be at least 32 characters. Never commit real secrets to version control. Use `TokenSettings__Secret` env var in production.

### Email — SendGrid

The application uses SendGrid for account activation emails. There is no mock fallback: **startup fails with an exception if `SENDGRID_API_KEY` is absent or empty**.

```env
SENDGRID_API_KEY=SG.xxxxxxxxxxxxxxxxxxxx
SENDGRID_FROM_EMAIL=noreply@yourdomain.com   # must be a verified sender
SENDGRID_FROM_NAME=EduSpace
FRONTEND_BASE_URL=https://app.yourdomain.com  # base URL for activation links
```

The activation email sends a link in the form `{FRONTEND_BASE_URL}/activate?token=<raw-token>`. The token is valid for 24 hours and is single-use.

### CORS

The current implementation allows all origins unconditionally:

```csharp
policy.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod()
```

`CORS_ALLOWED_ORIGINS` is present in `appsettings.json` but is not read by `Program.cs`. Origin restriction is a TODO.

---

## Development

### Build

```bash
dotnet clean
dotnet restore
dotnet build
dotnet build -c Release
```

### Hot reload

```bash
dotnet watch --project FULLSTACKFURY.EduSpace.API/FULLSTACKFURY.EduSpace.API.csproj
```

### Tests

```bash
# Run all tests
dotnet test FULLSTACKFURY.EduSpace.API.Tests/

# Run a specific class
dotnet test FULLSTACKFURY.EduSpace.API.Tests/ --filter "FullyQualifiedName~AccountCommandServiceSignInTests"

# Coverage
dotnet test FULLSTACKFURY.EduSpace.API.Tests/ --collect:"XPlat Code Coverage"
```

### Reset the database

```bash
# Drop and recreate via MySQL client
mysql -h localhost -P 3308 -u eduspace -p
DROP DATABASE IF EXISTS eduspacedb;
CREATE DATABASE eduspacedb;
# Then restart the API — migrations apply automatically

# Or via Docker (destroys all data)
docker-compose down -v
docker-compose up -d
```

### Adding a new entity

1. Create the aggregate root in `[Context]/Domain/Model/Aggregates/`
2. Add Commands and Queries in `[Context]/Domain/Model/Commands|Queries/`
3. Define the repository interface in `[Context]/Domain/Repositories/`
4. Implement Command and Query services in `[Context]/Application/Internal/`
5. Implement the repository in `[Context]/Infrastructure/Persistence/EFC/Repositories/`
6. Configure the entity in `Shared/Infrastructure/Persistence/EFC/Configuration/AppDbContext.cs`
7. Create the controller in `[Context]/Interfaces/REST/`
8. If the entity links to an IAM account, use the `AccountId` value object and be aware of the `account_ids` indirection table (see PROJECT.md — Known Issues)
9. Register all services in `Program.cs`
10. Add an EF migration: `dotnet ef migrations add AddYourEntity --project FULLSTACKFURY.EduSpace.API`

---

## Deployment

### Docker image

```bash
docker build -t eduspace-platform:latest .
docker run -d \
  -p 8080:8080 \
  --env-file .env \
  --name eduspace-api \
  eduspace-platform:latest
```

The image exposes port `8080`. `.env` is not baked into the image — inject all env vars at runtime.

### Azure Container Apps (current academic deployment)

- **API**: https://eduspace-api.purplemushroom-1f6e5ae3.brazilsouth.azurecontainerapps.io
- **Region**: Brazil South, consumption tier
- **Swagger**: disabled (production environment)
- **Database**: Aiven MySQL

Deploy from a local checkout:

```bash
./deploy.sh
```

The script builds a Docker image tagged with the current UTC timestamp, pushes to ACR (`ca1e77e4bf38acr`), and runs `az containerapp update`. See `deploy.sh` for override variables (`RG`, `APP_NAME`, etc.).

The application is also compatible with AWS Elastic Beanstalk, Google Cloud Run, Railway, and any Kubernetes cluster.

### Production environment variables

```env
ConnectionStrings__DefaultConnection=server=your-db-host;port=3306;user=eduspace;password=...;database=eduspacedb;AllowPublicKeyRetrieval=true;SslMode=Required

TokenSettings__Secret=your-production-secret-minimum-32-chars

SENDGRID_API_KEY=SG.your_production_key
SENDGRID_FROM_EMAIL=noreply@yourdomain.com
SENDGRID_FROM_NAME=EduSpace

FRONTEND_BASE_URL=https://app.yourdomain.com

ASPNETCORE_ENVIRONMENT=Production
```

---

## Contributing

1. Fork the repository and create a feature branch:
   ```bash
   git checkout -b feat/your-feature
   ```
2. Follow the coding standards:
   - Clean Architecture layer separation (Domain → Application → Infrastructure → Interfaces)
   - CQRS pattern for new features
   - No Infrastructure references from Domain
   - XML documentation on public APIs
3. Commit using [Conventional Commits](https://www.conventionalcommits.org/):
   - `feat:` — new feature
   - `fix:` — bug fix
   - `docs:` — documentation
   - `refactor:` — code cleanup
   - `test:` — adding or updating tests
4. Open a Pull Request against `main`.

---

## License

MIT License — Copyright (c) 2024 EduSpace Platform Contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
