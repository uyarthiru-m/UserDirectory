# User Directory

A full-stack user-management application with a React client and an ASP.NET Core Web API. Users can register with an email and password, sign in, and manage directory records. Accounts are stored in SQLite; authenticated API calls use short-lived JWT bearer tokens.

## Technology stack

- **Backend:** .NET 10, ASP.NET Core Web API, Entity Framework Core, SQLite
- **Authentication:** ASP.NET Core password hasher, JWT bearer authentication, ASP.NET Core rate limiting
- **Frontend:** React 19, React Router, Vite, Tailwind CSS
- **Tests and quality:** xUnit, Moq, `dotnet test`, Oxlint, Vite production build

## Repository structure

```text
backend/
  src/UserDirectory.Api/
    Controllers/    HTTP endpoints and response mapping
    Data/           EF Core context and database initialization
    DTOs/           API request and response contracts
    Models/         Persistence entities
    Repositories/   User data access
    Services/       User, local-account, and JWT business logic
  tests/UserDirectory.Tests/
    AuthServiceTests.cs
    UserServiceTests.cs
frontend/
  src/
    api/            HTTP clients for auth and users
    auth/           Browser authentication state and hook
    components/     Shared UI
    pages/          Login, user list, and add-user screens
```

## Requirements

- .NET 10 SDK
- Node.js and npm

## Configure and run locally

### 1. Configure the API signing key (once per development machine)

Open PowerShell at the repository root:

```powershell
Set-Location "D:\Console App\user-directory"

$bytes = New-Object 'Byte[]' 32
$rng = [Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($bytes)
$key = [Convert]::ToBase64String($bytes)
$rng.Dispose()

dotnet user-secrets set "Authentication:SigningKey" $key --project "backend\src\UserDirectory.Api\UserDirectory.Api.csproj"
```

The project already has a user-secrets ID. This saves the signing key outside the repository. Do not commit the key or place it in frontend configuration. The issuer and audience have development defaults in `appsettings.json`; override them through deployment configuration when needed.

### 2. Start the API

In the first terminal, from the repository root:

```powershell
dotnet run --project "backend\src\UserDirectory.Api\UserDirectory.Api.csproj" --launch-profile http
```

Keep the terminal open. The API listens at `http://localhost:5129`; its root redirects to Swagger in Development. If you see “address already in use,” an API instance is already running on that port. Reuse it or stop the existing instance before starting another.

### 3. Start the frontend

In a second terminal:

```powershell
Set-Location "D:\Console App\user-directory\frontend"
npm ci
npm run dev
```

Open the Vite URL, usually `http://localhost:5173`.

### 4. Register and use the app

1. Select **Create an account** and enter an email and password of at least 12 characters.
2. Registration signs you in automatically. On later visits, sign in with that email and password.
3. Use **List View** and **Add User** to manage directory records.

Both pages and the user API require sign-in. JWT access tokens expire after 45 minutes. Sign out clears the browser session. Registration and login are rate limited.

## Build, test, and lint

Run from the repository root:

```powershell
dotnet build "backend\UserDirectory.slnx"
dotnet test "backend\UserDirectory.slnx"
npm --prefix "frontend" run build
npm --prefix "frontend" run lint
```

Authentication tests use an isolated in-memory SQLite database and verify password hashing, normalized and duplicate emails, correct and incorrect credentials, and JWT claims/signature validation. User service tests cover DTO mapping and create, update, and delete behavior.

## Architecture and design decisions

- **Controller–service–repository:** Controllers handle HTTP concerns. Services implement authentication and user workflows. The user repository isolates EF Core data access.
- **DTO boundaries:** API request and response records keep persistence entities separate from HTTP contracts. ASP.NET Core validates request annotations before controller actions.
- **Dependency inversion:** Controllers depend on service interfaces; services receive persistence and token dependencies through constructor injection.
- **Single responsibility:** Account registration/password verification, JWT issuance/validation, user operations, database initialization, and browser authentication state are kept in focused classes/modules.
- **JWT security:** Passwords are stored with ASP.NET Core's salted password hasher, never as plaintext. The API validates JWT issuer, audience, signature, and lifetime. Keep the signing key private.
- **Database compatibility:** EF Core `EnsureCreated` initializes a new SQLite database. The database initializer also creates the account table/index if upgrading a pre-authentication development database. For production schema evolution, replace this compatibility initializer with versioned EF Core migrations.
- **Rate limiting:** Registration and login are limited to 10 attempts per minute per client IP in this single-instance development setup. Distributed deployments should use a shared rate-limit store and trusted proxy/IP configuration.
- **Scope:** This sample allows any registered account to manage the shared directory. It does not implement roles, per-account data ownership, email verification, password reset, refresh tokens, or account lockout.
- **Patterns are kept pragmatic:** Dependency injection and repository/service separation are used where they make dependencies replaceable and testable; no extra framework or abstraction layer is added for simple flows.

## AI tools used

GitHub Copilot was used to assist with implementation, refactoring, and documentation. Generated suggestions were reviewed, and the documented build, lint, test, and authentication checks were run against the project.

## Interview walkthrough

Start at `Program.cs` for dependency registration, JWT validation, middleware ordering, and startup initialization. Follow `AuthController` → `IAuthService`/`AuthService` → `IJwtTokenService` to explain local sign-in and token creation. Follow `UsersController` → `IUserService`/`UserService` → `IUserRepository`/`UserRepository` for directory operations. In the frontend, `AuthContext` holds the browser session, `Login` handles register/sign-in, and `userApi` attaches bearer tokens to protected requests. The corresponding service tests are in `backend/tests/UserDirectory.Tests`.
