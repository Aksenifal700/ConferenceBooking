# Conference Booking API

Backend assessment: manage conference rooms, find availability, book rooms with optional services, and view business reports.

## Stack and structure

.NET 10, ASP.NET Core controllers, EF Core, PostgreSQL 17, Identity + JWT, FluentValidation, OpenAPI / Swagger UI.

- Domain: entities and rental price calculation.
- Application: use cases, DTOs, repository/service contracts, business errors.
- DataAccess: EF mappings, migrations, repository implementations, Identity and JWT generation.
- Api: HTTP requests, mapping, controllers, authentication configuration and error middleware.

Domain has no project dependencies. Application references Domain; DataAccess references Application; Api composes the implementations through DI.

## Local setup (PowerShell)

Requirements: .NET 10 SDK, Docker with Compose. Run commands from the solution directory.

1. Copy the environment example and choose your own local database password:

```powershell
Copy-Item .env.example .env
# Edit POSTGRES_PASSWORD in .env before starting PostgreSQL.
docker compose up -d
docker compose exec postgres pg_isready -U postgres -d conference_booking
```

The database listens on localhost:5433. Compose starts PostgreSQL; the API runs with dotnet or Rider. The named volume preserves data across container restarts.

2. Configure local secrets (replace YOUR_PASSWORD with the same value as in .env):

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=conference_booking;Username=postgres;Password=YOUR_PASSWORD" --project ConferenceBooking.Api

$bytes = New-Object byte[] 32
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($bytes)
$rng.Dispose()
$jwtKey = [Convert]::ToBase64String($bytes)
dotnet user-secrets set "Jwt:SecretKey" $jwtKey --project ConferenceBooking.Api
```

The project already declares UserSecretsId. Do not commit secrets or .env. User Secrets are for local development; deployments can use ConnectionStrings__DefaultConnection and Jwt__SecretKey environment variables.

3. Restore, migrate, and launch:

```powershell
dotnet restore
# First installation only; if already installed, use dotnet tool update instead.
dotnet tool install --global dotnet-ef --version 10.0.12
dotnet ef database update --project ConferenceBooking.DataAccess --startup-project ConferenceBooking.Api
dotnet dev-certs https --trust
dotnet run --project ConferenceBooking.Api --launch-profile https
```

Swagger: https://localhost:7165/swagger
OpenAPI: https://localhost:7165/openapi/v1.json

Use POST /api/auth/register, then POST /api/auth/login. Passwords require at least 10 characters, a digit and an uppercase letter. Paste accessToken into Swagger's Authorize dialog without the Bearer prefix.

## API

| Method | Path | Purpose |
|---|---|---|
| POST | /api/auth/register | Register |
| POST | /api/auth/login | Get JWT |
| POST | /api/rooms | Create a room |
| GET | /api/rooms/{id} | Read an active room and available services |
| PUT | /api/rooms/{id} | Update a room and its complete list of services |
| DELETE | /api/rooms/{id} | Archive a room |
| GET | /api/rooms/available | Search by StartsAt, EndsAt and Capacity |
| POST | /api/bookings | Book a room |
| GET | /api/reports/revenue | Booking totals by room |
| GET | /api/reports/occupancy | Calendar occupancy by room |
| GET | /api/reports/services | Service popularity and totals |

Room reads/search and authentication endpoints are public. Room changes, bookings and reports require JWT. There are no roles: all authenticated accounts have the same permissions. This is the assessment's simplified access model, not a separation between customers and business administrators.

## Initial data

Migrations seed A (50 people, 2000 UAH/hour), B (100, 3500), C (30, 1500).
Room IDs respectively:

- aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa
- bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb
- cccccccc-cccc-4ccc-8ccc-cccccccccccc

Each room offers projector (500 UAH), Wi-Fi (300), sound (700). Services are optional and charged once per booking.

Service IDs respectively:

- 11111111-1111-4111-8111-111111111111
- 22222222-2222-4222-8222-222222222222
- 33333333-3333-4333-8333-333333333333

## Booking rules and calculation

All tariff hours are UTC. Bookings must be in the future and within 06:00–23:00 on one UTC calendar day. Partial hours are billed proportionally; intervals are split at tariff boundaries.

| Time | Multiplier |
|---|---|
| 06:00–09:00 | 0.90 |
| 09:00–12:00 | 1.00 |
| 12:00–14:00 | 1.15 |
| 14:00–18:00 | 1.00 |
| 18:00–23:00 | 0.80 |

Peak pricing overrides the standard tariff. Total = sum of timed segments + selected service prices, rounded to two decimals using AwayFromZero. Booking records preserve agreed prices, tariff segments and service names; changing the catalogue does not recalculate old bookings.

Example: room A, 10:00–15:00 = 2 × 2000 + 2 × 2000 × 1.15 + 1 × 2000 = 10,600 UAH. With projector: 11,100 UAH.

Booking request (choose a future date and an unoccupied interval):

```json
{
  "roomId": "aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa",
  "startsAt": "2026-12-15T10:00:00Z",
  "endsAt": "2026-12-15T15:00:00Z",
  "serviceIds": ["11111111-1111-4111-8111-111111111111"]
}
```

serviceIds contains UUID strings, not objects or client-supplied prices.

## Concurrency and archiving

Intervals are half-open: 10:00–11:00 and 11:00–12:00 do not overlap. A PostgreSQL exclusion constraint protects against concurrent overlapping inserts; it requires btree_gist, enabled by the migration.

Booking insertion and archiving lock the same room row in a transaction (FOR UPDATE). Archiving fails if a current or future booking exists. Archived rooms disappear from reads and availability, but remain in historical reports. Repositories return operation outcomes; application services translate those into business exceptions.

## Reports

Use from=2026-10-01&to=2026-11-01 for October. Dates are UTC, from inclusive, to exclusive; period length 1–366 days. See [report definitions](docs/reports.md).

Booking totals are not payment receipts. Occupancy is relative to 17 hours per calendar day; the model does not retain room creation/archive timestamps for historical capacity adjustment.

## Verification and migration notes

```powershell
dotnet build --configuration Release
dotnet ef migrations has-pending-model-changes --project ConferenceBooking.DataAccess --startup-project ConferenceBooking.Api
```

Manual checks: tariff boundaries and services; overlapping bookings (201 + 409); adjacent bookings; invalid request (400); missing room (404); missing token (401); archive with future bookings (409); empty-room archive (204); reports on known totals.

There is currently no automated test project. HTTP checks do not prove all concurrent schedules.

RemoveUnusedRoleTables removes the unused AspNetRoles, AspNetRoleClaims and AspNetUserRoles tables. User accounts and bookings are retained. Its rollback recreates empty role tables, not deleted role assignments. Do not remove already-applied migrations or reset the working database to clean migration history. An old development database may retain an entry for the deleted SeedIdentityRoles migration; this entry is not required on a clean install.

Swagger is enabled only in Development. Payments, cancellation, email, exports and role management are outside the implemented scope.
