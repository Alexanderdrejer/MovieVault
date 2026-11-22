# MovieVault API – SW4BAD Mandatory Assignment 4

ASP.NET Core Web API (.NET 10) with EF Core on SQL Server, ASP.NET Core Identity with JWT, movie reviews, a ticket sale simulator (worker service), request logging with Serilog to MongoDB, OpenAPI + Scalar, and Docker Compose.

## Group members and contributions

### Assignment 4

| Name | AU-ID | Contribution |
|---|---|---|
| Frederik Mosekjær Staal Pedersen | au765586 | |
| Malthe Winther | au741125 | |
| Andreas Østrem Pihl | au772149 | |
| Alexander Kjærsgaard Drejer | au555707 | Identity and JWT: ASP.NET Core Identity users and roles, seeded demo accounts, login endpoint, JWT creation and validation (401/403). Movie and query access: Identity connections for actors and directors, role-based authorization on the controllers, and Actor/Director movie filtering. |

### Assignment 3

| Name | AU-ID | Contribution |
|---|---|---|
| Frederik Mosekjær Staal Pedersen | au765586 | Project & solution setup, Docker Compose environment, `MoviesController` (full CRUD, Query 1 overview, Query 2 details, cast/location management), documentation and README. |
| Malthe Winther | au741125 | Consolidated E/R models (Chen notation & logical schema diagrams), domain entity models (`Domain/Models`), EF Core `MovieVaultContext` Fluent API configuration, relationships, and deletion constraints. |
| Andreas Østrem Pihl | au772149 | Initial database migration (`InitialCreate`), database seed data, request/response DTOs and validation rules, `TicketSalesController` (full CRUD, Query 4 ticket sales & revenue by movie, serializable capacity check). |
| Alexander Kjærsgaard Drejer | au555707 | `ScreeningsController` (Query 3 upcoming screenings, cancellation logic), `CinemasController`, `ActorsController` (Query 5 actor performance), `LocationsController`, and Serilog request logging middleware to MongoDB. |

## Solution structure

| Project | Description |
|---|---|
| `MovieVault.Api` | Web API: controllers, EF Core, Identity, JWT, request logging |
| `MovieVault.Simulator` | Worker service that simulates ticket sales through the API with `HttpClient` |

## Running with Docker Compose (recommended)

Requirements: Docker.

1. Copy the environment template and fill in local values (the file is git-ignored):

   ```bash
   cp .env.example .env
   ```

   | Variable | Used by | Meaning |
   |---|---|---|
   | `MSSQL_SA_PASSWORD` | mssql, api | SQL Server `sa` password (min. 8 chars, upper, lower, digit, symbol) |
   | `JWT_SIGNING_KEY` | api | Secret key used to sign and validate JWTs (at least 32 characters) |
   | `SIMULATOR_USERNAME` | api, simulator | User name of the simulator's dedicated Admin account |
   | `SIMULATOR_PASSWORD` | api, simulator | Password of the simulator account (upper, lower, digit, symbol) |

2. Start everything:

   ```bash
   docker compose up --build
   ```

| Service | Description |
|---|---|
| `mssql` | SQL Server 2022, data in volume `mssql-data` |
| `mongo` | MongoDB 8, request logs in volume `mongo-data` |
| `api` | MovieVault API on http://localhost:8080 |
| `simulator` | Ticket sale simulator, calls the API at `http://api:8080/` |

### Configuration

All secrets are supplied as environment variables from `.env`; no secrets are stored in source control. In `compose.yaml` the API gets `ConnectionStrings__MovieVault`, `Serilog__WriteTo__0__Args__databaseUrl`, `JWT__Issuer`, `JWT__Audience`, `JWT__SigningKey`, `Simulator__UserName` and `Simulator__Password`. A double underscore (`__`) maps to `:` in ASP.NET Core configuration, so `JWT__SigningKey` overrides `JWT:SigningKey` in `appsettings.json`.

### Networking

Containers reach each other through Docker's internal network using the service name as host name (`mssql`, `mongo`, `api`). `localhost` inside the simulator container refers to the simulator container itself, not to the API container, so the simulator uses `http://api:8080/`. The ports published in `compose.yaml` (`8080`, `1433`, `27017`) are only for access from the host machine.

### Startup readiness

- `mssql` and `mongo` have health checks (`SELECT 1` and `ping`).
- `api` uses `depends_on` with `condition: service_healthy`, so it starts only when both databases accept connections.
- `simulator` starts after `api` has started. If the API is not ready yet, the simulator logs a warning and retries every interval instead of terminating.

### Persistent storage

SQL Server data and MongoDB logs are stored in the named volumes `mssql-data` and `mongo-data`. `docker compose down` keeps the data; `docker compose down -v` deletes it.

## Running locally without Docker for the API

Start only the databases with `docker compose up -d mssql mongo` (requires `.env`), then set the local secrets once with user-secrets:

```bash
cd MovieVault.Api
dotnet user-secrets set "ConnectionStrings:MovieVault" "Data Source=127.0.0.1,1433;Database=MovieVault;User Id=sa;Password=<MSSQL_SA_PASSWORD>;TrustServerCertificate=True"
dotnet user-secrets set "JWT:SigningKey" "<at least 32 characters>"
dotnet user-secrets set "Simulator:UserName" "simulator"
dotnet user-secrets set "Simulator:Password" "<simulator password>"
dotnet run --launch-profile http

cd ../MovieVault.Simulator
dotnet user-secrets set "Simulator:UserName" "simulator"
dotnet user-secrets set "Simulator:Password" "<simulator password>"
dotnet run
```

Locally the API runs on http://localhost:5080 and the simulator uses `Api:BaseUrl` = `http://localhost:5080/` from its `appsettings.json`.

## URLs

| | Docker | Local |
|---|---|---|
| API base URL | http://localhost:8080/api | http://localhost:5080/api |
| Scalar UI | http://localhost:8080/scalar | http://localhost:5080/scalar |
| OpenAPI JSON | http://localhost:8080/openapi/v1.json | http://localhost:5080/openapi/v1.json |

In Scalar, log in with `POST /api/auth/login`, copy `accessToken` and paste it into the Bearer token field.

## Database, migrations and seeding

- The API calls `Database.Migrate()` on startup, so all EF Core migrations in `MovieVault.Api/Data/Migrations` are applied automatically (also in Docker).
- Migrations: `InitialCreate`, `AddIdentity` (Identity tables), `LinkActorDirectorToUser` (`UserId` on Actors and Directors), `AddReviewsAndMediaOutlets`.
- Domain seed data (movies, actors, screenings, media outlets, reviewers, reviews, …) is defined with `HasData` in `Data/SeedData.cs` and is part of the migrations.
- Roles, demo users and the simulator account are created on startup by `Data/IdentitySeeder.cs`, which also links the demo users to their Actor, Director and Reviewer rows. Existing roles and users are skipped, so restarting is safe.

Migrations can also be applied manually:

```bash
cd MovieVault.Api
dotnet ef database update
```

## Roles and demo accounts

All demo accounts use the password `Demo123!`.

| User name | Role | Linked to |
|---|---|---|
| `actor` | Actor | Actor 1, Elena Ruiz (appears in movies 1 and 2) |
| `director` | Director | Director 1, Greta Halvorsen (directed movies 1 and 2) |
| `reviewer` | Reviewer | Reviewer 1, Rita Reel (Aarhus Dagblad) |
| `reviewer2` | Reviewer | Reviewer 2, Jonas Frame (Kanal Nord Kultur) |
| `admin` | Admin | – |
| `SIMULATOR_USERNAME` | Admin | Dedicated simulator account (from `.env`) |

The Identity connection is a nullable, unique `UserId` foreign key from `Actors`, `Directors` and `Reviewers` to `AspNetUsers`. The server finds the logged-in person from the user id in the JWT (`NameIdentifier` claim); clients never send an actor, director or reviewer id to identify themselves.

## Authentication and authorization

- `POST /api/auth/login` validates the credentials with ASP.NET Core Identity (passwords are stored as salted hashes) and returns a JWT with the user id, user name and roles. Roles always come from the database, never from the request. Invalid credentials return 401.
- Every request with `Authorization: Bearer <token>` is validated (signature, issuer, audience and expiration).
- Protected requests without a valid token return **401**. Authenticated users without the required role or ownership return **403**.
- All controllers except `Auth` and `Reviews` require authentication. Writes to movies, screenings and ticket sales require the Admin role.

### Movie access (`Services/MovieAccessService.cs`)

| Role | Movies |
|---|---|
| Actor | Movies the actor appears in |
| Director | Movies the director has directed |
| Reviewer, Admin | All movies |

Collection endpoints (`GET /api/movies`, `/api/movies/ticket-sales`, `/api/screenings/upcoming`, `/api/cinemas/{id}/screenings`, `GET /api/ticketsales`) filter out movies outside the user's scope and return 200 (an empty list if the person has no movies). Requests for one specific movie outside the scope (`GET /api/movies/{id}`, `/api/movies/{id}/screenings`, `/api/screenings/{id}`, `/api/ticketsales/{id}`) return 403. Ticket sales by movie is denied for Reviewers; actor performance is allowed for Admin (any actor) and for an Actor (own performance only).

**Multiple roles:** if an account has several roles, the movie lists of the roles are combined. Admin or Reviewer always gives access to all movies.

## Reviews and media outlets

- `MediaOutlet` (name, type, website), `Reviewer` (name, media outlet, `UserId`) and `Review` (headline, text, rating 1–5, publication date, movie, reviewer, media outlet).
- Normalization: reviewer and media outlet information is stored once and referenced by foreign keys. The review stores the media outlet the reviewer represented when the review was published.
- Reading is public: `GET /api/reviews` (optional `?movieId=` and `?mediaOutletId=` query parameters) and `GET /api/reviews/{id}`.
- `POST /api/reviews`: Reviewers create reviews as themselves; the author and media outlet are determined from the logged-in user. Admin must give `reviewerId` in the body.
- `PUT` / `DELETE /api/reviews/{id}`: Reviewers can only change their own reviews (403 otherwise); Admin can change all reviews.
- Media outlets are seed data only (full CRUD is not required).

## Ticket sale simulator

`MovieVault.Simulator` is a worker service (`TicketSaleSimulator`, a `BackgroundService`). Every `Simulator:IntervalSeconds` (default 15) it:

1. logs in with the simulator account (once, and again if a request returns 401),
2. fetches `GET /api/screenings/upcoming` (screening and movie information),
3. picks a random screening with free seats and buys 1–4 tickets at the screening's price with `POST /api/ticketsales`.

The simulator only talks to the API, never to the databases, so simulated sales follow the same business rules (validation, capacity check) and show up in the existing sales queries. Network errors, timeouts and error responses are logged and the simulator continues with the next interval.

| Setting | Meaning |
|---|---|
| `Api:BaseUrl` | API address (`http://api:8080/` in Docker) |
| `Simulator:UserName`, `Simulator:Password` | Credentials of the dedicated Admin account |
| `Simulator:IntervalSeconds` | Seconds between simulated sales |

## Request logs

A middleware in `Program.cs` logs every POST, PUT and DELETE request, including requests rejected by authentication, authorization or validation, through Serilog to MongoDB (`movievaultlogs` database, `log` collection). Each entry contains `Username`, `Timestamp` (UTC), `RequestType`, `Route` and `StatusCode`. The username is `anonymous` for public endpoints (login, reviews) and for requests without an authenticated user. Request bodies, passwords, tokens, headers and connection strings are never logged. GET requests are not logged.

`GET /api/logs` (Admin only) returns the request logs, newest first, with optional filters that can be combined; empty or omitted filters are ignored:

| Query parameter | Filter |
|---|---|
| `username` | Exact user name |
| `date` | Day in UTC, format `yyyy-MM-dd` |
| `requestType` | HTTP method, e.g. `POST` |
| `route` | Route prefix, case-insensitive, e.g. `/api/movies` |

## Endpoints

| Method | Route | Access |
|---|---|---|
| POST | /api/auth/login | Anonymous |
| GET | /api/movies | Authenticated, filtered by role |
| GET | /api/movies/{id} | Authenticated, 403 outside scope |
| POST / PUT / DELETE | /api/movies, /api/movies/{id} | Admin |
| POST / DELETE | /api/movies/{id}/actors, /api/movies/{id}/locations | Admin |
| GET | /api/movies/{id}/screenings | Authenticated, 403 outside scope |
| GET | /api/movies/ticket-sales | Actor, Director, Admin (filtered) |
| GET | /api/screenings/upcoming | Authenticated, filtered |
| GET | /api/screenings/{id} | Authenticated, 403 outside scope |
| POST / PUT / DELETE | /api/screenings | Admin |
| GET | /api/cinemas, /api/cinemas/{id}/screenings | Authenticated (screenings filtered) |
| GET | /api/ticketsales, /api/ticketsales/{id} | Authenticated, filtered / 403 outside scope |
| POST / PUT / DELETE | /api/ticketsales | Admin |
| GET | /api/actors | Authenticated |
| GET | /api/actors/{id}/performance | Admin, Actor (own) |
| GET | /api/locations | Authenticated |
| GET | /api/reviews, /api/reviews/{id} | Anonymous |
| POST | /api/reviews | Reviewer, Admin |
| PUT / DELETE | /api/reviews/{id} | Reviewer (own), Admin |
| GET | /api/logs | Admin |

## Business rules

- Ticket count must be at least 1 and prices cannot be negative.
- A ticket sale cannot make a screening exceed the seating capacity of its screen. The check runs in a serializable transaction.
- Tickets cannot be sold for a cancelled screening or one that has started.
- A new screening must start in the future and refer to an existing movie and screen.
- A screening cannot be moved to a screen with fewer seats than the tickets already sold.
- Movies and screenings with dependent sales data cannot be deleted (409 Conflict).
- An actor or location can only be added once per movie (409 Conflict).
- Cancelling a screening (`PUT` with `isCancelled: true`) stops new sales and hides it from upcoming screenings; its existing sales are kept and still count in the sales queries.
- Updating a ticket sale re-checks capacity without counting the sale itself. Deleting a ticket sale frees its seats again and removes its revenue from the sales queries; the movie-sales query sums ticket quantities, not sale records.
- Simulated sales use the same endpoint and therefore the same rules as manual sales.

## Postman tests

`postman/MovieVault.postman_collection.json` tests the Movie controller; `postman/MovieVault.postman_environment.template.json` is the environment template.

1. Import both files in Postman and select the environment.
2. Fill in `adminPassword` and `actorPassword` (`Demo123!`). Set `baseUrl` to `http://localhost:8080` (Docker) or `http://localhost:5080` (local).
3. Run the collection in order (Collection Runner):
   1. **Login** – Admin and Actor tokens are stored in `adminToken` / `actorToken`.
   2. **Authentication** – `GET /api/movies` and `GET /api/movies/{id}` without token return 401.
   3. **Movie access** – Admin sees all movies; Actor sees movie `actorMovieId` but not `otherMovieId`.
   4. **Movie CRUD** – Admin POST (201), GET by id (200), PUT (204), GET shows the update.
   5. **Authorization** – Actor POST, PUT and DELETE return 403.
   6. **Cleanup** – Admin DELETE (204), GET returns 404.

The created movie is deleted at the end, so the collection can be run again.

## Demonstration order

1. `docker compose up --build`, open Scalar.
2. `GET /api/movies` without token → 401.
3. Log in as `actor` → `GET /api/movies` returns movies 1 and 2; `GET /api/movies/3` → 403.
4. Log in as `admin` → `GET /api/movies` returns all movies.
5. `GET /api/reviews?movieId=1` without token → public reviews from two media outlets; `reviewer2` updating review 1 (by `reviewer`) → 403.
6. Show simulator output (`docker compose logs -f simulator`) and the new sales in `GET /api/movies/ticket-sales`.
7. As `admin`: `GET /api/logs?username=simulator&requestType=POST`.
8. Run the Postman collection.

## E/R diagram

The diagrams are in `docs/`:

- `ER_MovieVault_chen` – conceptual model (Chen notation)
- `ER_MovieVault_schema` – logical model (crow's foot) as implemented with EF Core. Tables added in Assignment 4 (Identity tables, `UserId` connections, `Reviewer`, `Review`, `MediaOutlet`) are marked in purple.
