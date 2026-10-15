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

## Build and Test Screenshots
<img width="1366" height="768" alt="Screenshot 2026-10-15 081626" src="https://github.com/user-attachments/assets/b146dc32-684a-4aca-9089-4f66f5e5b8f0" />
docs: add CI green build screenshot

```



