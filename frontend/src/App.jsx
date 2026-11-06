import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import UserList from './pages/UserList';
import AddUser from './pages/AddUser';

export default function App() {
  return (
    <Router>
      <div className="min-h-screen bg-slate-100 text-slate-900">
        <Navbar />
        <main className="p-4">
          <Routes>
            <Route path="/" element={<UserList />} />
            <Route path="/add" element={<AddUser />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}