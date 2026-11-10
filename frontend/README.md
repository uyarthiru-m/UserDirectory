# Frontend

The React/Vite client for User Directory. It provides the local email/password sign-in and registration page, protected directory routes, and authenticated API requests.

From the frontend directory, install dependencies and start the development server:

```powershell
npm ci
npm run dev
```

Open the URL Vite prints (usually `http://localhost:5173`). The backend must also be running at `http://localhost:5129`. No OIDC provider or frontend secret is required for local accounts.

For the complete application setup, architecture, and test/build commands, see the [repository README](../README.md).
