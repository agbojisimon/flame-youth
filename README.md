# Global Flame Ministry API

Global Flame Ministry API is an ASP.NET Core Web API (targeting .NET 8) powering the backend for Global Flame Ministry. It provides authentication (Identity + JWT), content management (announcements, sermons, events, books), user accounts, donations, counselling requests, bulk email, and background email scheduling.

## Quick links
- Project: .NET 8 Web API
- DB: PostgreSQL (Npgsql + EF Core)
- API docs: Swagger (enabled in Program.cs)
- Procfile: present for Heroku-style deployment

## Prerequisites
- .NET SDK 8.0
- PostgreSQL (for local development or a connection string to a hosted instance)
- Optional: dotnet-ef (for migrations), dotnet-format (for formatting)

## Build, run & publish
- Restore & build:
  - dotnet build
- Run locally (development):
  - dotnet run
  - The app reads PORT environment variable (Program.cs). Default fallback is 8080.
  - Example (PowerShell): $env:PORT = 5000; dotnet run
- Publish for deployment:
  - dotnet publish -c Release -o ./bin/publish
  - The included Procfile uses: `web: cd bin/publish && ./GlobalFlameMinistry.API --urls http://*:$PORT`

## Database & EF Core
- Project uses Entity Framework Core with Npgsql provider.
- Add migration:
  - dotnet ef migrations add <Name>
- Apply migrations:
  - dotnet ef database update
- On startup the app calls `context.Database.MigrateAsync()` and runs `DataSeeder.SeedAdminAsync` (see Program.cs).

## Tests
- There are no test projects in the repository. When tests are added, run:
  - dotnet test
- To run a single test when tests exist (example):
  - dotnet test --filter "FullyQualifiedName=Your.Namespace.YourTestClass.YourTestMethod"

## Lint / Format
- No linter is configured by default. Recommended tooling:
  - dotnet tool install -g dotnet-format
  - dotnet format

## Configuration & Secrets
Important configuration keys (set via appsettings, environment variables, or user-secrets):
- ConnectionStrings:DefaultConnection — PostgreSQL connection string
- JWT:SigningKey, JWT:Issuer, JWT:Audience, JWT:ExpiryMinutes
- AdminSeed:Email, AdminSeed:Password — seeding admin credentials
- EmailSettings (Server, Port, SenderEmail, Password)
- Brevo:ApiKey — for Brevo (SendinBlue) integration

Example environment variable names (Linux/PowerShell dotnet configuration binding):
- ConnectionStrings__DefaultConnection
- JWT__SigningKey
- AdminSeed__Email
- AdminSeed__Password
- Brevo__ApiKey

User secrets are enabled (UserSecretsId present in the csproj) and are recommended for local development of sensitive values.

## Architecture (high-level)
- Composition root: Program.cs
  - Configures DI, authentication (Identity + JWT), database context, hosted services, global filters, CORS, and Swagger.
- Layers and folders:
  - Controllers/ — API endpoints grouped by area (Admin, Ministry, Youth, Auth, Account)
  - Services/ — business logic and orchestration
  - Repository/ & Repository files — data access (EF Core interactions)
  - DTOs/ & Mappers/ — request/response shapes and mapping
  - Data/ — AppDbContext and DataSeeder
  - Filters/ — ApiResponseFilter, GlobalExceptionFilter, ValidationFilter (registered globally)
  - Helpers/ — QueryObject classes for filtering/pagination and other shared helpers
- Background work: EmailSchedulerService (hosted service) + EmailSender and Brevo HttpClient integration

## Conventions and patterns
- Interface-first DI: Interfaces are under Interfaces/ and implementations under Services/ or Repository/. Prefer injecting interfaces.
- Repository + Service pairing per domain (e.g., IEventRepository + EventRepository and IEventService + EventService).
- DTO + Mapper pattern: Use DTOs for external shapes and Mappers for conversion between domain models and DTOs.
- Query objects: Classes under Helpers implement consistent filtering & pagination behavior.
- Global filters handle validation, exception shaping, and API response formatting — avoid duplicating that logic.
- Identity: Email confirmation is required and a strict password policy is configured in Program.cs.

## Observability & API docs
- Swagger / OpenAPI is configured via Swashbuckle and available when the app runs (Program.cs registers Swagger UI).

## Deployment notes
- The app respects a PORT environment variable and contains a Procfile for Heroku-like deployments.
- Ensure required secrets (DB connection, JWT signing key, email creds) are set in the deployment environment.

## Where to start when contributing
1. Read Program.cs to understand services, DI, and global behavior.
2. Inspect Data/AppDbContext.cs and Migrations/ for schema and migrations.
3. Add or update DTO + Mappers when changing API shapes.
4. Follow existing Service + Repository patterns for new domains.

---

If you need a CONTRIBUTING.md, CI workflow, or templates added, open an issue or submit a PR. Contributions and documentation updates are welcome.

---

### Example .env (local development)

# PostgreSQL connection string
ConnectionStrings__DefaultConnection="Host=localhost;Database=globalflame;Username=postgres;Password=changeme"

# JWT / Identity
JWT__SigningKey="<your-very-long-secret>"
JWT__Issuer="GlobalFlameMinistryAPI"
JWT__Audience="GlobalFlameMinistryClient"
JWT__ExpiryMinutes=60

# Admin seeding (used by startup DataSeeder)
AdminSeed__Email="admin@example.com"
AdminSeed__Password="P@ssw0rd!"

# SMTP / Email settings (used by EmailSender)
EmailSettings__Server="smtp.example.com"
EmailSettings__Port=587
EmailSettings__SenderName="Global Flame"
EmailSettings__SenderEmail="noreply@globalflameministry.org"
EmailSettings__Email="smtp-username"
EmailSettings__Password="smtp-password"

# Brevo / SendinBlue API key (optional if using Brevo client)
Brevo__ApiKey="your-brevo-api-key"

# Optional app settings
App__FrontendUrl="http://localhost:5173"
APPSETTING__PORT=8080

> Notes: Use user-secrets for local development where possible (`dotnet user-secrets set`). Environment variable names use double-underscore (__) to denote nesting for .NET configuration binding.

---

### API endpoints

Auth
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/refresh-token
- GET  /api/auth/confirm-email?userId={id}&code={code}
- POST /api/auth/resend-confirmation
- POST /api/auth/forgot-password
- POST /api/auth/reset-password

Account (Authenticated)
- GET  /api/account/me
- PUT  /api/account/me
- POST /api/account/me/profile-picture
- GET  /api/account/me/prayer-requests
- GET  /api/account/me/registrations
- GET  /api/account/me/donations
- POST /api/account/me/request-email-change
- POST /api/account/me/confirm-email-change

Public Ministry
- GET  /api/ministry/sermons
- GET  /api/ministry/sermons/{id}
- GET  /api/ministry/announcements
- GET  /api/ministry/announcements/{id}
- GET  /api/ministry/events
- GET  /api/ministry/events/{id}
- POST /api/ministry/events/{id}/register
- POST /api/ministry/donations/paystack
- POST /api/ministry/donations/flutterwave
- GET  /api/ministry/donations/verify/paystack?reference={{ref}}
- GET  /api/ministry/donations/verify/flutterwave?transaction_id={{id}}
- POST /api/ministry/counselling
- POST /api/ministry/books
- GET  /api/ministry/books
- GET  /api/ministry/books/{id}
- POST /api/ministry/prayer-requests
- GET  /api/prayerrequest/track/{token}
- POST /api/testimony (anonymous or authenticated)

Youth (authenticated / some admin actions)
- POST /api/youth/join (authenticated)
- GET  /api/youth/announcements
- GET  /api/youth/announcements/{id}
- POST /api/youth/announcements (Admin)
- PUT  /api/youth/announcements/{id} (Admin)
- DELETE /api/youth/announcements/{id} (Admin)
- GET  /api/youth/events (authenticated)
- GET  /api/youth/events/{id}
- POST /api/youth/events/{id}/register
- POST /api/youth/events (Admin)
- PUT  /api/youth/events/{id} (Admin)
- DELETE /api/youth/events/{id} (Admin)

Admin (requires Admin role)
- GET  /api/admin/users
- GET  /api/admin/users/{id}
- DELETE /api/admin/users/{id}
- POST /api/admin/users/assign-role

- GET  /api/admin/announcements
- GET  /api/admin/announcements/{id}
- POST /api/admin/announcements
- PUT  /api/admin/announcements/{id}
- DELETE /api/admin/announcements/{id}

- GET  /api/admin/sermons (use AdminSermonController)
- GET  /api/admin/sermons/{id}
- POST /api/admin/sermons
- PUT  /api/admin/sermons/{id}
- DELETE /api/admin/sermons/{id}

- GET  /api/admin/events
- GET  /api/admin/events/{id}
- GET  /api/admin/events/{id}/registrations
- POST /api/admin/events
- PUT  /api/admin/events/{id}
- DELETE /api/admin/events/{id}

- GET  /api/admin/donations
- GET  /api/admin/donations/{id}
- GET  /api/admin/donations/stats

- GET  /api/admin/books
- GET  /api/admin/books/{id}
- POST /api/admin/books
- PUT  /api/admin/books/{id}
- DELETE /api/admin/books/{id}

- GET  /api/admin/bulk-email/history
- GET  /api/admin/bulk-email/stats
- POST /api/admin/bulk-email/send
- POST /api/admin/bulk-email/schedule
- DELETE /api/admin/bulk-email/{id}

- GET  /api/admin/contacts
- GET  /api/admin/contacts/{id}
- PATCH /api/admin/contacts/{id}/status
- DELETE /api/admin/contacts/{id}

- GET  /api/admin/testimonies
- GET  /api/admin/testimonies/{id}
- PATCH /api/admin/testimonies/{id}/status
- DELETE /api/admin/testimonies/{id}
- GET  /api/admin/testimonies/all

- GET  /api/admin/prayer-requests
- GET  /api/admin/prayer-requests/{id}
- PATCH /api/admin/prayer-requests/{id}/attend
- DELETE /api/admin/prayer-requests/{id}/permanent

- GET  /api/admin/ministries
- GET  /api/admin/ministries/{id}
- POST /api/admin/ministries
- PUT  /api/admin/ministries/{id}
- DELETE /api/admin/ministries/{id}

- GET  /api/admin/counselling
- GET  /api/admin/counselling/{id}
- PUT  /api/admin/counselling/{id}/status
- DELETE /api/admin/counselling/{id}

---

If you need a CONTRIBUTING.md, CI workflow, or templates added, open an issue or submit a PR. Contributions and documentation updates are welcome.

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>
