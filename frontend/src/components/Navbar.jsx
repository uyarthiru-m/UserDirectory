import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../auth/useAuth';

export default function Navbar() {
  const { signOut, user } = useAuth();

  return (
    <header className="bg-slate-900 text-white shadow-md">
      <div className="max-w-5xl mx-auto px-4 py-4 flex justify-between items-center">
        <span className="text-xl font-bold tracking-tight">User Directory</span>
        <nav className="flex items-center gap-6 text-sm font-medium">
          <Link to="/" className="hover:text-blue-400 transition-colors">List View</Link>
          <Link to="/add" className="hover:text-blue-400 transition-colors">Add User</Link>
          <span className="text-slate-300">{user?.email}</span>
          <button type="button" onClick={signOut} className="hover:text-blue-400 transition-colors">
            Sign out
          </button>
        </nav>
      </div>
    </header>
  );
}