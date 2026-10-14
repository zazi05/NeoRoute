# NeoRoute RaceDay API – Part 2 Skeleton

Complete ASP.NET Core 8 Web API matching your ERD + expanded API plan.

## Features implemented

- **Authentication** – Register / Login / Logout / Me (cookie-based session)
- **Role-based access** – Organiser vs Participant enforced with `[Authorize(Roles = "...")]`
- **Password hashing** – BCrypt
- **Profile** – View & update own profile
- **Events** – Full CRUD (Organiser only for write)
- **Categories** – Full CRUD (Organiser only for write)
- **Enrolments** – Participants enrol, Organisers view all, cancel support
- **Results** – Organisers capture finish times & positions, Participants view own
- **Swagger** – Fully documented endpoints at `/swagger`
- **EF Core Code-First** – Schema matches the fixed `RaceDayDB.sql`

## Quick start

1. Open `NeoRoute.API.csproj` in **Visual Studio 2022**.
2. Update the connection string in `appsettings.json` if needed (default uses LocalDB).
3. Press **F5** (or run). Swagger UI opens automatically.
4. Test flow:
   - `POST /api/auth/register` → create an Organiser and a Participant
   - `POST /api/auth/login` → session cookie is set
   - Use the lock icon in Swagger or just call protected endpoints

## Project structure

```
NeoRoute.API/
├── Controllers/          Auth, Profile, Events, Categories, Enrolments, Results
├── Models/               All entities from the ERD + User
├── DTOs/                 Request/response models
├── Data/RaceDayContext.cs
├── Program.cs            Auth, EF, Swagger configuration
└── appsettings.json
```


1. Run the project and take screenshots of Swagger + successful calls.
2. Add unit tests (xUnit + WebApplicationFactory) covering the required scenarios.
3. Add `.github/workflows/ci.yml` that builds + runs tests.
4. Make ≥ 20 meaningful commits on a Part2 branch.
5. Update the main README with setup instructions, roles description, CI green screenshot and YouTube link.
