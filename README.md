# 🎓 EduSpace Platform

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?style=for-the-badge&logo=mysql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**A comprehensive educational management platform built with .NET 8 following Clean Architecture and Domain-Driven Design principles.**

[Features](#-features) •
[Architecture](#-architecture) •
[Quick Start](#-quick-start) •
[API Documentation](#-api-documentation) •
[Contributing](#-contributing)

</div>

---

## 📋 Table of Contents

- [About](#-about)
- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Prerequisites](#-prerequisites)
- [Quick Start](#-quick-start)
- [API Documentation](#-api-documentation)
- [Project Structure](#-project-structure)
- [Configuration](#-configuration)
- [Development](#-development)
- [Deployment](#-deployment)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🎯 About

**EduSpace Platform** is an enterprise-grade educational management system designed to streamline the administration of educational institutions. Built as a modular monolith, it provides high cohesion and low coupling between components while maintaining simplicity in deployment and operations.

The platform manages everything from user authentication to space reservations, meeting scheduling, and maintenance reporting—all through a robust, scalable RESTful API.

---

## ✨ Features

### 🔐 Identity & Access Management (IAM)
- **JWT access tokens** (short-lived, default 60 min) with **rotating refresh tokens** (default 14 days)
- **Two-Factor Authentication (2FA)** via email verification (6-digit code, 10 min expiry)
- **BCrypt password hashing** for maximum security
- **Role-based authorization** (Administrator, Teacher)
- Strict JWT validation: `ValidateIssuer`, `ValidateAudience`, `ValidateIssuerSigningKey` all enabled
- Email verification via [Resend](https://resend.com) HTTP API (falls back to a mock service that logs codes to stdout when `RESEND_API_KEY` is not set)

### 👥 Profile Management
- **Teacher profiles** with comprehensive information
- **Administrator profiles** with specialized permissions
- Full CRUD operations for profile management
- Profile linking with authentication accounts

### 🏫 Spaces & Resource Management
- **Classroom management** with detailed configurations
- **Shared area tracking** (libraries, auditoriums, etc.)
- **Resource inventory** (equipment, materials, facilities)
- Availability tracking and status management

### 📅 Reservation Scheduling
- **Meeting scheduling** with teacher participation
- **Many-to-many meeting sessions** support via `MeetingParticipants`
- Teacher availability management
- Meeting audit trail for compliance

### 🔧 Breakdown Management
- Report maintenance issues and breakdowns
- Track repair status and history
- Resource and space incident logging
- Priority-based issue management

---

## 🏗️ Architecture

EduSpace follows **Clean Architecture** principles with **Domain-Driven Design (DDD)** patterns, organized into clear bounded contexts:

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

Each context is independently developed with its own domain model:

| Context | Responsibility |
|---------|---------------|
| **IAM** | Authentication, Authorization, User Management |
| **Profiles** | Teacher & Administrator Profile Management |
| **SpacesAndResourceManagement** | Classroom, Resource, Shared Area Management |
| **ReservationScheduling** | Meeting Planning & Teacher Participation |
| **BreakdownManagement** | Maintenance & Incident Reporting |

### Key Design Patterns

- **CQRS** (Command Query Responsibility Segregation)
- **Repository Pattern** with Unit of Work
- **Anti-Corruption Layer (ACL)** for inter-context communication
- **Value Objects** for domain concepts
- **Aggregate Roots** for consistency boundaries
- **Dependency Injection** throughout all layers

---

## 🛠️ Tech Stack

### Backend
- **.NET 8.0** - Modern C# framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 8** - ORM for data access
- **MySQL 8.0** - Relational database

### Security
- **JWT (JSON Web Tokens)** - Stateless authentication
- **BCrypt.Net-Next** - Password hashing
- **Microsoft.IdentityModel.Tokens** - Token validation

### Email & Communication
- **Resend** - Email delivery service (called via `HttpClient`, no SDK dependency)

### Documentation & Tools
- **Swagger/OpenAPI** - Interactive API documentation
- **Swashbuckle** - Swagger generator for .NET
- **Docker** - Containerization
- **DotNetEnv** - Environment variable management

### Libraries & Utilities
- **Humanizer** - Database naming conventions
- **EntityFrameworkCore.CreatedUpdatedDate** - Automatic audit timestamps

---

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [MySQL 8.0](https://dev.mysql.com/downloads/) or higher
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional, for containerized development)
- [Git](https://git-scm.com/downloads)

**Recommended IDE:**
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.8+)
- [JetBrains Rider](https://www.jetbrains.com/rider/)
- [Visual Studio Code](https://code.visualstudio.com/) with C# extension

---

## 🚀 Quick Start

### Option 1: Local Development Setup

#### 1. Clone the Repository

```bash
git clone https://github.com/DesarrolloSolucionIoT/eduspace-platform.git
cd eduspace-platform
```

#### 2. Configure Environment Variables

Copy the `.env.example` file and configure your environment variables:

```bash
cp .env.example .env
```

Edit the `.env` file with your credentials:

```env
# Database
MYSQL_ROOT_PASSWORD=your_root_password
MYSQL_DATABASE=eduspacedb
MYSQL_USER=eduspace
MYSQL_PASSWORD=your_mysql_password
MYSQL_PORT=3308

ConnectionStrings__DefaultConnection=server=localhost;port=3308;user=eduspace;password=your_mysql_password;database=eduspacedb;AllowPublicKeyRetrieval=true;SslMode=none

# Email — Resend (leave RESEND_API_KEY empty to fall back to MockEmailService that logs codes to stdout)
RESEND_API_KEY=
RESEND_FROM=onboarding@resend.dev
RESEND_FROM_NAME=EduSpace Platform

# CORS
CORS_ALLOWED_ORIGINS=http://localhost:5173,http://localhost:3000,http://localhost:4200
```

#### 3. Configure Application Settings

The `appsettings.json` file contains default values. Real credentials should be set in the `.env` file, which will override these settings

#### 4. Start MySQL Database

```bash
# Start MySQL with Docker Compose
docker-compose up -d
```

#### 5. Restore Dependencies

```bash
dotnet restore
```

#### 6. Run the Application

```bash
dotnet run --project FULLSTACKFURY.EduSpace.API/FULLSTACKFURY.EduSpace.API.csproj
```

The API will be available at:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:5001/swagger`

### Option 2: Docker Development

#### 1. Create `.env` file (as shown above)

#### 2. Start Services

```bash
docker-compose up --build
```

This will:
- Start a MySQL 8.0 container
- Build and run the .NET application
- Automatically create the database schema
- Expose the API on port 8080

#### 3. Access the Application

- **API**: `http://localhost:8080`
- **Swagger**: `http://localhost:8080/swagger`

---

## 📚 API Documentation

### Interactive Documentation

Once the application is running, visit **Swagger UI** for interactive API documentation:

```
https://localhost:5001/swagger
```

### Authentication Flow

#### 1. Sign Up
```http
POST /api/v1/authentication/sign-up
Content-Type: application/json

{
  "email": "teacher@example.com",
  "password": "SecurePass123!",
  "role": "teacher"
}
```

#### 2. Sign In (Request Verification Code)
```http
POST /api/v1/authentication/sign-in
Content-Type: application/json

{
  "email": "teacher@example.com",
  "password": "SecurePass123!"
}
```

Response: Verification code sent to email

#### 3. Verify Code (Get JWT Token)
```http
POST /api/v1/authentication/verify-code
Content-Type: application/json

{
  "email": "teacher@example.com",
  "code": "123456"
}
```

Response:
```json
{
  "id": 1,
  "email": "teacher@example.com",
  "role": "teacher",
  "profileId": 5,
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "f3a9b6c2...",
  "accessTokenExpiresIn": 3600
}
```

#### 4. Use Token in Requests

Add the access token to the `Authorization` header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
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

### Core Endpoints

#### 👥 Teacher Profiles
```http
GET    /api/v1/teachers-profiles           # Get all teachers
GET    /api/v1/teachers-profiles/{id}      # Get teacher by ID
POST   /api/v1/teachers-profiles            # Create teacher profile
PUT    /api/v1/teachers-profiles/{id}      # Update teacher profile
DELETE /api/v1/teachers-profiles/{id}      # Delete teacher profile
```

#### 👨‍💼 Administrator Profiles
```http
GET    /api/v1/administrator-profiles      # Get all administrators
GET    /api/v1/administrator-profiles/{id} # Get administrator by ID
POST   /api/v1/administrator-profiles      # Create admin profile
PUT    /api/v1/administrator-profiles/{id} # Update admin profile
DELETE /api/v1/administrator-profiles/{id} # Delete admin profile
```

#### 🏫 Classrooms
```http
GET    /api/v1/classrooms                  # Get all classrooms
GET    /api/v1/classrooms/{id}             # Get classroom by ID
POST   /api/v1/classrooms                  # Create classroom
PUT    /api/v1/classrooms/{id}             # Update classroom
DELETE /api/v1/classrooms/{id}             # Delete classroom
```

#### 📦 Resources
```http
GET    /api/v1/resource                    # Get all resources
GET    /api/v1/resource/{id}               # Get resource by ID
POST   /api/v1/resource                    # Create resource
PUT    /api/v1/resource/{id}               # Update resource
DELETE /api/v1/resource/{id}               # Delete resource
```

#### 🏛️ Shared Areas
```http
GET    /api/v1/sharedarea                  # Get all shared areas
GET    /api/v1/sharedarea/{id}             # Get shared area by ID
POST   /api/v1/sharedarea                  # Create shared area
PUT    /api/v1/sharedarea/{id}             # Update shared area
DELETE /api/v1/sharedarea/{id}             # Delete shared area
```

#### 📅 Meetings
```http
GET    /api/v1/meeting                     # Get all meetings
GET    /api/v1/meeting/{id}                # Get meeting by ID
GET    /api/v1/meeting/teacher/{teacherId} # Get meetings by teacher
POST   /api/v1/meeting                     # Create meeting
PUT    /api/v1/meeting/{id}                # Update meeting
DELETE /api/v1/meeting/{id}                # Delete meeting
```

#### 👥 Meeting Participants
```http
GET    /api/v1/meeting-participants                          # Get all participants
GET    /api/v1/meeting-participants/meeting/{meetingId}      # Participants by meeting
POST   /api/v1/meeting-participants                          # Add participant
DELETE /api/v1/meeting-participants/{id}                     # Remove participant
```

#### 🔧 Reports (Breakdowns)
```http
GET    /api/v1/report                      # Get all reports
GET    /api/v1/report/{id}                 # Get report by ID
POST   /api/v1/report                      # Create report
PUT    /api/v1/report/{id}                 # Update report
DELETE /api/v1/report/{id}                 # Delete report
```

---

## 📂 Project Structure

```
eduspace-platform/
├── FULLSTACKFURY.EduSpace.API/
│   ├── BreakdownManagement/
│   │   ├── Domain/
│   │   │   ├── Model/
│   │   │   │   ├── Aggregates/         # Report
│   │   │   │   ├── Commands/           # CQRS commands
│   │   │   │   └── Queries/            # CQRS queries
│   │   │   ├── Repositories/           # Repository interfaces
│   │   │   └── Services/               # Domain service interfaces
│   │   ├── Application/
│   │   │   └── Internal/
│   │   │       ├── CommandServices/    # Command handlers
│   │   │       └── QueryServices/      # Query handlers
│   │   ├── Infrastructure/
│   │   │   └── Persistence/EFC/
│   │   │       └── Repositories/       # EF Core implementations
│   │   └── Interface/
│   │       └── REST/
│   │           ├── Resources/          # DTOs
│   │           ├── Transform/          # DTO assemblers
│   │           └── ReportController.cs
│   │
│   ├── IAM/                             # Identity & Access Management
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   │   ├── Hashing/BCrypt/
│   │   │   ├── Tokens/JWT/
│   │   │   ├── Services/               # EmailService
│   │   │   └── Pipeline/Middleware/    # JWT middleware
│   │   └── Interfaces/
│   │       ├── REST/
│   │       └── ACL/                    # Anti-Corruption Layer
│   │
│   ├── Profiles/                        # User Profiles
│   ├── ReservationScheduling/           # Meetings & Participants
│   ├── SpacesAndResourceManagement/     # Facilities
│   │
│   ├── Shared/                          # Cross-cutting concerns
│   │   ├── Domain/
│   │   │   └── Repositories/           # IBaseRepository
│   │   └── Infrastructure/
│   │       └── Persistence/EFC/
│   │           ├── Configuration/      # AppDbContext
│   │           └── Repositories/       # BaseRepository
│   │
│   ├── Program.cs                       # Application entry point
│   ├── appsettings.json                 # Configuration
│   └── FULLSTACKFURY.EduSpace.API.csproj
│
├── docker-compose.yml                   # Docker services
├── Dockerfile                           # Container definition
├── .env.example                         # Environment template
└── README.md                            # This file
```

---

## ⚙️ Configuration

### Database Configuration

The application uses **EF Core migrations** applied automatically on startup via `context.Database.Migrate()`. Migrations live under `FULLSTACKFURY.EduSpace.API/Migrations/`.

To add a new migration after changing the model:

```bash
dotnet ef migrations add YourMigrationName --project FULLSTACKFURY.EduSpace.API
```

**Connection String Format:**
```
server=localhost;port=3308;user=eduspace;password=your_password;database=eduspacedb;AllowPublicKeyRetrieval=true;SslMode=none
```

**Note:** Set this in your `.env` file using the variable `ConnectionStrings__DefaultConnection`

### JWT Token Settings

JWT settings live under `TokenSettings` in `appsettings.json`:

```json
{
  "TokenSettings": {
    "Secret": "your-jwt-secret-key-here-minimum-32-characters-recommended",
    "Issuer": "EduSpace.API",
    "Audience": "EduSpace.Clients",
    "AccessTokenLifetimeMinutes": 60,
    "RefreshTokenLifetimeDays": 14
  }
}
```

**Important**:
- Secret must be at least 32 characters; rotate in production via env vars
- `Issuer` and `Audience` are validated on every request — keep clients in sync
- Never commit real secrets to version control

### Email Configuration

For 2FA email verification, the backend uses [Resend](https://resend.com) via its HTTP API. The DI registration in `Program.cs` swaps the implementation based on whether `RESEND_API_KEY` is configured:

- `RESEND_API_KEY` set → real `EmailService` (sends via Resend)
- `RESEND_API_KEY` empty or absent → `MockEmailService` (logs the 6-digit code to stdout — handy for local development without burning emails)

```env
RESEND_API_KEY=re_xxxxxxxxxxxxxxxxxxxxx
RESEND_FROM=onboarding@resend.dev      # use the Resend sandbox sender, or a verified address from your domain
RESEND_FROM_NAME=EduSpace Platform
```

**Free-tier limitation:** without a verified domain in Resend, emails can only be delivered to the address registered on the Resend account. Add and verify your own domain (Settings → Domains) to send to arbitrary recipients.

### CORS Configuration

Configure allowed origins in your `.env` file:

```env
# Multiple origins (comma-separated)
CORS_ALLOWED_ORIGINS=http://localhost:5173,http://localhost:3000,http://localhost:4200

# Production example
CORS_ALLOWED_ORIGINS=https://app.example.com,https://www.example.com

# Leave empty to allow all origins (development only)
CORS_ALLOWED_ORIGINS=
```

---

## 💻 Development

### Building the Project

```bash
# Clean solution
dotnet clean

# Restore dependencies
dotnet restore

# Build
dotnet build

# Build release configuration
dotnet build -c Release
```

### Running with Hot Reload

```bash
dotnet watch --project FULLSTACKFURY.EduSpace.API/FULLSTACKFURY.EduSpace.API.csproj
```

### Database Management

**Reset Database:**
```sql
-- Connect to MySQL (adjust port if needed)
mysql -h localhost -P 3308 -u eduspace -p

-- Drop and recreate
DROP DATABASE IF EXISTS eduspacedb;
CREATE DATABASE eduspacedb;
```

Then restart the application to auto-create schema.

**Using Docker:**
```bash
# Stop and remove MySQL container (WARNING: This deletes all data)
docker-compose down -v

# Start fresh
docker-compose up -d
```

### Adding a New Entity

1. **Create Domain Model** in `[Context]/Domain/Model/Aggregates/`
2. **Add Commands/Queries** in `[Context]/Domain/Model/Commands|Queries/`
3. **Define Repository Interface** in `[Context]/Domain/Repositories/`
4. **Create Command/Query Services** in `[Context]/Application/Internal/`
5. **Implement Repository** in `[Context]/Infrastructure/Persistence/EFC/Repositories/`
6. **Configure Entity** in `Shared/Infrastructure/Persistence/EFC/Configuration/AppDbContext.cs`
7. **Create Controller** in `[Context]/Interfaces/REST/`
8. **Register Services** in `Program.cs`
9. **Add an EF migration**: `dotnet ef migrations add AddYourEntity --project FULLSTACKFURY.EduSpace.API`

### Dependency Injection Pattern

Register new services in `Program.cs`:

```csharp
// Repository
builder.Services.AddScoped<IYourEntityRepository, YourEntityRepository>();

// Command Service
builder.Services.AddScoped<IYourEntityCommandService, YourEntityCommandService>();

// Query Service
builder.Services.AddScoped<IYourEntityQueryService, YourEntityQueryService>();
```

---

## 🚢 Deployment

### Docker Deployment

Build and run with Docker:

```bash
# Build image
docker build -t eduspace-platform:latest .

# Run container
docker run -d \
  -p 8080:8080 \
  --env-file .env \
  --name eduspace-api \
  eduspace-platform:latest
```

### Docker Compose Production

```bash
docker-compose -f docker-compose.prod.yml up -d
```

### Cloud Deployment

**Current academic deployment** (Brazil South):
- API: https://eduspace-api.purplemushroom-1f6e5ae3.brazilsouth.azurecontainerapps.io (Azure Container Apps, consumption tier)
- Swagger is disabled in Production
- Database: Aiven MySQL (replacement provisioned 2026-05-19 after the original trial was decommissioned)
- Re-deploy from a local checkout: `./deploy.sh` (docker build → ACR push → `az containerapp update`)

The application is also compatible with:
- **AWS Elastic Beanstalk**
- **Google Cloud Run**
- **Heroku** / **Railway**
- Any **Kubernetes cluster**

### Environment Variables for Production

Ensure these are configured in your hosting environment:

```env
# Database
MYSQL_ROOT_PASSWORD=your_production_root_password
MYSQL_DATABASE=eduspacedb
MYSQL_USER=eduspace
MYSQL_PASSWORD=your_production_password
MYSQL_PORT=3306

ConnectionStrings__DefaultConnection=server=your-db-host;port=3306;user=eduspace;password=your_production_password;database=eduspacedb;AllowPublicKeyRetrieval=true;SslMode=Required

# Email — Resend
RESEND_API_KEY=re_your_production_key
RESEND_FROM=noreply@yourdomain.com         # must be a verified sender on a verified domain
RESEND_FROM_NAME=EduSpace Platform

# CORS
CORS_ALLOWED_ORIGINS=https://yourdomain.com,https://www.yourdomain.com
```

---

## 🤝 Contributing

We welcome contributions! Please follow these steps:

### 1. Fork the Repository

```bash
git clone https://github.com/your-username/eduspace-platform.git
cd eduspace-platform
```

### 2. Create a Feature Branch

```bash
git checkout -b feature/amazing-feature
```

### 3. Make Your Changes

Follow the coding standards:
- Use Clean Architecture principles
- Follow CQRS pattern for new features
- Add XML documentation to public APIs
- Write unit tests for business logic
- Update API documentation in Swagger

### 4. Commit Your Changes

```bash
git add .
git commit -m "feat: add amazing feature"
```

Use conventional commits:
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `refactor:` Code refactoring
- `test:` Adding tests
- `chore:` Maintenance tasks

### 5. Push and Create Pull Request

```bash
git push origin feature/amazing-feature
```

Then open a Pull Request on GitHub.

### Code Style Guidelines

- Follow [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Keep methods small and focused
- Maintain layer separation (Domain → Application → Infrastructure → Interface)
- Never reference Infrastructure from Domain layer

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2024 EduSpace Platform Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
```
