import {
  BrowserRouter as Router,
  Navigate,
  Outlet,
  Route,
  Routes,
  useLocation,
} from 'react-router-dom';
import { AuthProvider } from './auth/AuthContext';
import { useAuth } from './auth/useAuth';
import Navbar from './components/Navbar';
import UserList from './pages/UserList';
import AddUser from './pages/AddUser';
import Login from './pages/Login';

function RequireAuth() {
  const { user } = useAuth();
  const location = useLocation();

  if (!user) {
    return <Navigate to="/login" replace state={{ returnTo: location.pathname }} />;
  }
  return <Outlet />;
}

function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route element={<RequireAuth />}>
        <Route element={<><Navbar /><main className="p-4"><Outlet /></main></>}>
          <Route path="/" element={<UserList />} />
          <Route path="/add" element={<AddUser />} />
        </Route>
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default function App() {
  return (
    <Router>
      <AuthProvider>
        <div className="min-h-screen bg-slate-100 text-slate-900">
          <AppRoutes />
        </div>
      </AuthProvider>
    </Router>
  );
}