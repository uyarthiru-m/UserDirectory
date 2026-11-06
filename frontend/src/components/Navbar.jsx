import React from 'react';
import { Link } from 'react-router-dom';

export default function Navbar() {
  return (
    <header className="bg-slate-900 text-white shadow-md">
      <div className="max-w-5xl mx-auto px-4 py-4 flex justify-between items-center">
        <span className="text-xl font-bold tracking-tight">User Directory</span>
        <nav className="space-x-6 text-sm font-medium">
          <Link to="/" className="hover:text-blue-400 transition-colors">List View</Link>
          <Link to="/add" className="hover:text-blue-400 transition-colors">Add User</Link>
        </nav>
      </div>
    </header>
  );
}