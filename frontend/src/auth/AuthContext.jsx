import { useCallback, useEffect, useMemo, useState } from 'react';
import { loginAccount, registerAccount } from '../api/authApi';
import { AuthContext } from './context';

function readSavedSession() {
  const saved = window.sessionStorage.getItem('user-directory-session');
  if (!saved) return null;

  try {
    const session = JSON.parse(saved);
    if (session.accessToken && Date.parse(session.expiresAtUtc) > Date.now()) return session;
  } catch {
    window.sessionStorage.removeItem('user-directory-session');
    return null;
  }

  window.sessionStorage.removeItem('user-directory-session');
  return null;
}

export function AuthProvider({ children }) {
  const [session, setSession] = useState(readSavedSession);

  const saveSession = useCallback((newSession) => {
    window.sessionStorage.setItem('user-directory-session', JSON.stringify(newSession));
    setSession(newSession);
  }, []);

  const login = useCallback(async (credentials) => {
    const result = await loginAccount(credentials);
    saveSession(result);
  }, [saveSession]);

  const register = useCallback(async (credentials) => {
    const result = await registerAccount(credentials);
    saveSession(result);
  }, [saveSession]);

  const signOut = useCallback(() => {
    window.sessionStorage.removeItem('user-directory-session');
    setSession(null);
  }, []);

  useEffect(() => {
    window.addEventListener('auth:expired', signOut);
    return () => window.removeEventListener('auth:expired', signOut);
  }, [signOut]);

  const getAccessToken = useCallback(() => {
    if (session?.accessToken && Date.parse(session.expiresAtUtc) > Date.now()) {
      return session.accessToken;
    }

    signOut();
    throw new Error('Your session expired. Please sign in again.');
  }, [session, signOut]);

  const value = useMemo(() => ({
    user: session ? { email: session.email } : null,
    login,
    register,
    signOut,
    getAccessToken,
  }), [session, login, register, signOut, getAccessToken]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
