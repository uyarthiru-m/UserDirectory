import { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/useAuth';

export default function Login() {
  const { login, register } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
  const [creatingAccount, setCreatingAccount] = useState(false);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(null);
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError(null);
    setSubmitting(true);

    try {
      const credentials = { email: email.trim(), password };
      if (creatingAccount) {
        await register(credentials);
      } else {
        await login(credentials);
      }

      const returnTo = location.state?.returnTo;
      navigate(returnTo?.startsWith('/') && !returnTo.startsWith('//') ? returnTo : '/', {
        replace: true,
      });
    } catch (authError) {
      setError(authError.message);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <main className="mx-auto mt-16 max-w-md rounded-lg border bg-white p-8 shadow-sm">
      <h1 className="text-2xl font-semibold text-slate-800">
        {creatingAccount ? 'Create your account' : 'Sign in to User Directory'}
      </h1>
      <p className="mt-2 text-sm text-slate-600">
        Use your email and password to {creatingAccount ? 'create an account' : 'sign in'}.
      </p>
      {error && (
        <div role="alert" className="mt-5 rounded border border-red-200 bg-red-50 p-3 text-sm text-red-700">
          {error}
        </div>
      )}
      <form onSubmit={handleSubmit} className="mt-6 space-y-4">
        <div>
          <label htmlFor="email" className="block text-sm font-medium text-slate-700">Email</label>
          <input
            id="email"
            type="email"
            autoComplete="email"
            required
            maxLength={254}
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            className="mt-1 w-full rounded border px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
          />
        </div>
        <div>
          <label htmlFor="password" className="block text-sm font-medium text-slate-700">Password</label>
          <input
            id="password"
            type="password"
            autoComplete={creatingAccount ? 'new-password' : 'current-password'}
            required
            minLength={12}
            maxLength={128}
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            className="mt-1 w-full rounded border px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
          />
          {creatingAccount && (
            <p className="mt-1 text-xs text-slate-500">Use at least 12 characters.</p>
          )}
        </div>
        <button
          type="submit"
          disabled={submitting}
          className="w-full rounded bg-blue-600 px-4 py-2 font-medium text-white transition-colors hover:bg-blue-700 disabled:bg-blue-300"
        >
          {submitting ? 'Please wait...' : creatingAccount ? 'Create account' : 'Sign in'}
        </button>
      </form>
      <p className="mt-5 text-center text-sm text-slate-600">
        {creatingAccount ? 'Already have an account?' : 'New here?'}{' '}
        <button
          type="button"
          onClick={() => {
            setCreatingAccount(!creatingAccount);
            setError(null);
          }}
          className="font-medium text-blue-700 hover:underline"
        >
          {creatingAccount ? 'Sign in' : 'Create an account'}
        </button>
      </p>
    </main>
  );
}
