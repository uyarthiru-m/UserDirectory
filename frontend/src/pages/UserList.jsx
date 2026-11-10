import React, { useEffect, useState } from 'react';
import { fetchUsers } from '../api/userApi';
import { useAuth } from '../auth/useAuth';

export default function UserList() {
  const { getAccessToken } = useAuth();
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    getAccessToken()
      .then(fetchUsers)
      .then(setUsers)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [getAccessToken]);

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4 my-4 bg-red-50 border border-red-200 text-red-700 rounded-md">
        <strong>Error:</strong> {error}
      </div>
    );
  }

  return (
    <div className="max-w-5xl mx-auto mt-8 p-6 bg-white shadow-sm rounded-lg border">
      <h2 className="text-2xl font-semibold mb-6 text-slate-800">All Registered Users</h2>
      {users.length === 0 ? (
        <p className="text-slate-500 text-center py-8">No users found. Add your first user using the navigation above.</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse text-sm text-slate-700">
            <thead className="bg-slate-50 border-b">
              <tr>
                <th className="py-3 px-4 font-semibold">Name</th>
                <th className="py-3 px-4 font-semibold">Age</th>
                <th className="py-3 px-4 font-semibold">City</th>
                <th className="py-3 px-4 font-semibold">State</th>
                <th className="py-3 px-4 font-semibold">Pincode</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {users.map((u) => (
                <tr key={u.id} className="hover:bg-slate-50">
                  <td className="py-3 px-4 font-medium text-slate-900">{u.name}</td>
                  <td className="py-3 px-4">{u.age}</td>
                  <td className="py-3 px-4">{u.city}</td>
                  <td className="py-3 px-4">{u.state}</td>
                  <td className="py-3 px-4">{u.pincode}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}