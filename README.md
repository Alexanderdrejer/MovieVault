# MovieVault API – SW4BAD Mandatory Assignment 3

ASP.NET Core Web API (.NET 10) with EF Core on SQL Server, request logging with Serilog to MongoDB, and OpenAPI + Scalar documentation.

## Group members and contributions

| Name | AU-ID | Contribution |
|---|---|---|
| Frederik Mosekjær Staal Pedersen | au765586 | Project & solution setup, Docker Compose environment, `MoviesController` (full CRUD, Query 1 overview, Query 2 details, cast/location management), documentation and README. |
| Malthe Winther | au741125 | Consolidated E/R models (Chen notation & logical schema diagrams), domain entity models (`Domain/Models`), EF Core `MovieVaultContext` Fluent API configuration, relationships, and deletion constraints. |
| Andreas Østrem Pihl | au772149 | Initial database migration (`InitialCreate`), database seed data, request/response DTOs and validation rules, `TicketSalesController` (full CRUD, Query 4 ticket sales & revenue by movie, serializable capacity check). |
| Alexander Kjærsgaard Drejer | au555707 | `ScreeningsController` (Query 3 upcoming screenings, cancellation logic), `CinemasController`, `ActorsController` (Query 5 actor performance), `LocationsController`, and Serilog request logging middleware to MongoDB. |

## Running the application

Requirements: .NET 10 SDK, Docker and the EF Core CLI (`dotnet tool install --global dotnet-ef`).

Start SQL Server and MongoDB:

```bash
docker compose up -d
```

Start the API:

```bash
cd MovieVault.Api
dotnet run --launch-profile http
```

## Database and migrations

The database is created from the EF Core migrations in `MovieVault.Api/Data/Migrations`. The API calls `Database.Migrate()` on startup, so running the API creates the `MovieVault` database and inserts the seed data. The seed data is defined with `HasData` in `Data/SeedData.cs` and is part of the `InitialCreate` migration.

The migrations can also be applied manually:

```bash
cd MovieVault.Api
dotnet ef database update
```

Connection string (`appsettings.json`, matches `compose.yaml`):

```
Data Source=127.0.0.1,1433;Database=MovieVault;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True
```

## URLs

| | URL |
|---|---|
| API base URL | http://localhost:5080/api |
| Scalar UI | http://localhost:5080/scalar |
| OpenAPI JSON | http://localhost:5080/openapi/v1.json |

## Endpoints

| Method | Route | Description |
|---|---|---|
| GET | /api/movies | Movie overview: title, release date, status, director, production company, budget |
| GET | /api/movies/{id} | Movie details: director, production record, cast with roles, filming locations |
| POST | /api/movies | Create movie |
| PUT | /api/movies/{id} | Update movie |
| DELETE | /api/movies/{id} | Delete movie (409 if it has screenings) |
| POST | /api/movies/{id}/actors | Assign actor with character name and role type |
| DELETE | /api/movies/{id}/actors/{actorId} | Remove actor from cast |
| POST | /api/movies/{id}/locations | Associate filming location with shooting days |
| DELETE | /api/movies/{id}/locations/{locationId} | Remove filming location |
| GET | /api/movies/{id}/screenings | Screenings for a movie |
| GET | /api/movies/ticket-sales | Tickets sold and revenue per movie |
| GET | /api/screenings/upcoming | Upcoming screenings in chronological order |
| GET | /api/screenings/{id} | One screening |
| POST | /api/screenings | Create screening for a movie on a screen |
| PUT | /api/screenings/{id} | Update or cancel (`isCancelled`) a screening |
| DELETE | /api/screenings/{id} | Delete screening (409 if it has ticket sales) |
| GET | /api/cinemas | Cinemas with screens |
| GET | /api/cinemas/{id}/screenings | Screenings for a cinema |
| GET | /api/ticketsales | All ticket sales |
| GET | /api/ticketsales/{id} | One ticket sale |
| POST | /api/ticketsales | Register ticket sale |
| PUT | /api/ticketsales/{id} | Update ticket count and price |
| DELETE | /api/ticketsales/{id} | Delete ticket sale |
| GET | /api/actors | Actors |
| GET | /api/actors/{id}/performance | Movies the actor appears in and their ticket sales |
| GET | /api/locations | Filming locations |

## Business rules

- Ticket count must be at least 1 and prices cannot be negative.
- A ticket sale cannot make a screening exceed the seating capacity of its screen. The check runs in a serializable transaction.
- Tickets cannot be sold for a cancelled screening or one that has started.
- A new screening must start in the future and refer to an existing movie and screen.
- A screening cannot be moved to a screen with fewer seats than the tickets already sold.
- Movies and screenings with dependent sales data cannot be deleted (409 Conflict).
- An actor or location can only be added once per movie (409 Conflict).
- Status, role type and shooting dates are validated in the DTOs and the controllers.

## Logging

Every POST, PUT and DELETE request is logged by a middleware in `Program.cs` with method, path, status code and timestamp. Serilog writes the entries to MongoDB (`movievaultlogs` database, `log` collection) through the MongoDB sink, configured in the `Serilog` section of `appsettings.json`.

```bash
docker compose exec mongo mongosh movievaultlogs --eval 'db.log.find({"Properties.LogInfo": {$exists: true}}).sort({_id: -1}).limit(5)'
```

## E/R diagram

The consolidated diagrams are in `docs/`:

- `ER_MovieVault_chen` – conceptual model (Chen notation)
- `ER_MovieVault_schema` – logical model (crow's foot) as implemented with EF Core
