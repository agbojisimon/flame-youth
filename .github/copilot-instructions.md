# Copilot instructions for GlobalFlameMinistry.API

This file is intended to help future Copilot CLI sessions and contributors quickly understand how to build, run, and reason about this repository.

---

## Build / Run / Publish (commands)
- Build the app:
  - dotnet build
- Run locally (development):
  - dotnet run
  - The app binds to PORT if provided; default port used in Program.cs is 8080. To override: $env:PORT=5000; dotnet run
- Publish for deployment (used by Procfile):
  - dotnet publish -c Release -o ./bin/publish
  - Procfile: `web: cd bin/publish && ./GlobalFlameMinistry.API --urls http://*:$PORT`

## Database / EF Core
- Migrations tools are referenced in the csproj (Microsoft.EntityFrameworkCore.Tools).
- Create a migration:
  - dotnet ef migrations add <Name>
- Apply migrations to DB:
  - dotnet ef database update
- On startup the app auto-applies pending migrations and runs DataSeeder.SeedAdminAsync (see Program.cs).

## Tests
- No test projects are present in this repository. If/when test projects are added, run:
  - dotnet test
- Example to run a single test by name (when tests exist):
  - dotnet test --filter "FullyQualifiedName=Your.Namespace.YourTestClass.YourTestMethod"

## Lint / Format
- No explicit linter is configured. dotnet-format can be used if installed:
  - dotnet tool install -g dotnet-format
  - dotnet format

---

## High-level architecture (big picture)
- This is an ASP.NET Core Web API targeting .NET 8 (net8.0).
- Persistence: Entity Framework Core with Npgsql (PostgreSQL). AppDbContext lives in Data folder; Migrations are present under Migrations/.
- Authentication: ASP.NET Core Identity with JWT Bearer tokens. JWT configuration (Issuer/Audience/SigningKey/Expiry) is provided via configuration (appsettings/user secrets/environment).
- Composition root: Program.cs — configures DI, services, repositories, hosted services, global filters, Swagger, CORS, and auto-migrations + seeding.
- Layering pattern used across codebase:
  - Controllers: HTTP endpoints (Controllers/*, Admin, Ministry, Youth, Account, Auth namespaces).
  - Services: Business logic and orchestration (Services/*).
  - Repositories: Data access abstractions and EF Core interactions (Repository/ or Repository files).
  - DTOs + Mappers: DTOs grouped by area (DTOs/*) and mapping helpers in Mappers/*.
  - Filters: Global API filters (GlobalExceptionFilter, ValidationFilter, ApiResponseFilter) applied application-wide in Program.cs.
- Background work: EmailSchedulerService is registered as a hosted service (IHostedService). Email sending uses Brevo API via HttpClient and local EmailSender implementation.
- App auto-seed: DataSeeder.SeedAdminAsync runs on startup (see Program.cs) — admin credentials are read from configuration keys (AdminSeed).
- Deployment notes: Program.cs reads PORT environment variable (Heroku-style). Procfile present for process declaration.

---

## Key conventions and repository-specific patterns
- Dependency injection: Interfaces live under Interfaces/* and concrete implementations under Services/ or Repository/. Prefer injecting interfaces.
- Repository + Service pairing: For each domain area (e.g., Sermons, Events), there is I<X>Repository + <X>Repository and I<X>Service + <X>Service. Follow this pattern when adding new domains.
- DTO / Mapper pattern: All external shapes (request/response) use DTOs in DTOs/* and mapping logic in Mappers/*. Keep business models separate from DTOs.
- Query objects: Filtering/pagination uses QueryObject classes under Helpers (e.g., SermonQueryObject). Add query objects for consistent query parameter handling.
- Global filters: Validation, exception handling, and response shaping are centralized via ApiResponseFilter, ValidationFilter, GlobalExceptionFilter (registered in Program.cs). Avoid duplicating logic already handled by filters.
- Serializer config: NewtonsoftJson is used and configured to ignore reference loops — rely on these settings for serializing EF navigation properties.
- Password & Identity rules: Identity is configured with strict password policy and RequireConfirmedEmail=true. Seeded users must match those constraints.
- Secrets & configuration:
  - Sensitive values are expected to be set via appsettings.Development.json, environment variables, or user-secrets (UserSecretsId present in csproj).
  - Important keys: ConnectionStrings:DefaultConnection, JWT:SigningKey, AdminSeed:Email & AdminSeed:Password, EmailSettings, Brevo.ApiKey.
- Hosted services and HttpClient registration:
  - AddHostedService is used for scheduling (EmailSchedulerService).
  - External HTTP clients (Brevo) use builder.Services.AddHttpClient with a named or typed client.

---

## Where to look first when editing/adding features
- Composition and wiring: Program.cs
- Database model & migrations: Data/AppDbContext.cs and Migrations/
- Business logic: Services/ and Repositories/
- API shapes and mapping: DTOs/ and Mappers/
- Global behavior: Filters/ and Helpers/
- Seeding and startup DB tasks: Data/DataSeeder.cs

---

If you update project structure, DI bindings, or the seeding/migration behaviour, update this file so future Copilot sessions and contributors get correct guidance.

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>
