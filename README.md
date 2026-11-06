# User Directory Application

A full-stack User Management application built with a .NET 10 Web API backend, React frontend, and SQLite database.

## Technology Stack
- **Backend:** .NET 10 Web API, Entity Framework Core, SQLite, xUnit, Moq
- **Frontend:** React (Vite), React Router DOM, Tailwind CSS, Vitest
- **Containerization:** Docker & Docker Compose

## Architecture & Design Decisions
- **SOLID & Separation of Concerns:** Implements Controller-Service-Repository pattern. Decouples business logic from HTTP endpoints and persistence layers.
- **DTO Layer:** Strict boundary separation between EF Core internal entity models and API payloads.
- **Dependency Inversion:** All services and repositories are injected via standard .NET dependency injection.
- **Client & Server Validation:** Both layers enforce identical constraints (Name length 2–100, Age 0–120, Pincode length 4–10).

## AI Tools Utilized
- **GitHub Copilot & AI Chat Assistants:** Leveraged to scaffold boilerplate DTOs, suggest EF Core repository contracts, generate initial test fixtures, and optimize Tailwind CSS form layouts.

## Setup & Execution

### Prerequisites
- .NET 10 SDK
- Node.js and npm

### Run locally
Start the backend and frontend in separate terminals. Keep both terminals open while using the app.

#### 1. Start the backend
From the repository root, run:

```powershell
dotnet run --project "backend\src\UserDirectory.Api\UserDirectory.Api.csproj" --launch-profile http
```

The backend listens at `http://localhost:5129`. You can check the API at
`http://localhost:5129/swagger`.

#### 2. Start the frontend
In a second terminal, run:

```powershell
cd frontend
npm ci
npm run dev
```

Open the local URL printed by Vite, usually `http://localhost:5173`.

#### 3. View and add users
- Select **List View** to see saved users. A new database starts empty, so the list initially displays “No users found.”
- Select **Add User**, enter a name (2–100 characters), age (0–120), city, state, and pincode (4–10 characters), then submit.
- After a successful save, the app returns to **List View**, where the new user should appear.

The frontend calls the backend at `http://localhost:5129/api/users`. If the list or submit reports an error, make sure the backend terminal is still running at that address.
