import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createUser } from '../api/userApi';
import { useAuth } from '../auth/useAuth';

export default function AddUser() {
  const navigate = useNavigate();
  const { getAccessToken } = useAuth();
  const [formData, setFormData] = useState({
    name: '',
    age: '',
    city: '',
    state: '',
    pincode: '',
  });
  const [errors, setErrors] = useState({});
  const [submitting, setSubmitting] = useState(false);
  const [apiError, setApiError] = useState(null);

  const validate = () => {
    const errs = {};
    if (!formData.name.trim() || formData.name.length < 2 || formData.name.length > 100) {
      errs.name = 'Name must be between 2 and 100 characters.';
    }
    const ageNum = parseInt(formData.age, 10);
    if (formData.age === '' || isNaN(ageNum) || ageNum < 0 || ageNum > 120) {
      errs.age = 'Age must be an integer between 0 and 120.';
    }
    if (!formData.city.trim()) errs.city = 'City is required.';
    if (!formData.state.trim()) errs.state = 'State is required.';
    if (!formData.pincode.trim() || formData.pincode.length < 4 || formData.pincode.length > 10) {
      errs.pincode = 'Pincode must be between 4 and 10 characters.';
    }
    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    if (errors[e.target.name]) setErrors({ ...errors, [e.target.name]: null });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setApiError(null);
    if (!validate()) return;

    setSubmitting(true);
    try {
      const token = await getAccessToken();
      await createUser({ ...formData, age: parseInt(formData.age, 10) }, token);
      navigate('/', { state: { toastMessage: 'User successfully created!' } });
    } catch (err) {
      setApiError(err.message);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="max-w-xl mx-auto mt-8 p-6 bg-white shadow-sm rounded-lg border">
      <h2 className="text-2xl font-semibold mb-6 text-slate-800">Create New User</h2>
      {apiError && <div className="mb-4 p-3 bg-red-50 text-red-700 text-sm rounded border border-red-200">{apiError}</div>}
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-slate-700">Name</label>
          <input
            type="text"
            name="name"
            value={formData.name}
            onChange={handleChange}
            className="mt-1 w-full rounded border px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
          />
          {errors.name && <span className="text-xs text-red-500 mt-1 block">{errors.name}</span>}
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700">Age</label>
          <input
            type="number"
            name="age"
            value={formData.age}
            onChange={handleChange}
            className="mt-1 w-full rounded border px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
          />
          {errors.age && <span className="text-xs text-red-500 mt-1 block">{errors.age}</span>}
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700">City</label>
          <input
            type="text"
            name="city"
            value={formData.city}
            onChange={handleChange}
            className="mt-1 w-full rounded border px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
          />
          {errors.city && <span className="text-xs text-red-500 mt-1 block">{errors.city}</span>}
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700">State</label>
          <input
            type="text"
            name="state"
            value={formData.state}
            onChange={handleChange}
            className="mt-1 w-full rounded border px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
          />
          {errors.state && <span className="text-xs text-red-500 mt-1 block">{errors.state}</span>}
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700">Pincode</label>
          <input
            type="text"
            name="pincode"
            value={formData.pincode}
            onChange={handleChange}
            className="mt-1 w-full rounded border px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
          />
          {errors.pincode && <span className="text-xs text-red-500 mt-1 block">{errors.pincode}</span>}
        </div>
        <button
          type="submit"
          disabled={submitting}
          className="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 rounded transition-colors disabled:bg-blue-300"
        >
          {submitting ? 'Submitting...' : 'Submit User'}
        </button>
      </form>
    </div>
  );
}