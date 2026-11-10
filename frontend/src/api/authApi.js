const BASE_URL = `${import.meta.env.VITE_API_URL || 'http://localhost:5129'}/api/auth`;

async function postCredentials(endpoint, credentials) {
  const response = await fetch(`${BASE_URL}/${endpoint}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    const body = await response.text();
    if (body) {
      let errorData;
      try {
        errorData = JSON.parse(body);
      } catch {
        throw new Error('Authentication failed.');
      }
      throw new Error(errorData.message || errorData.title || 'Authentication failed.');
    }
    throw new Error('Authentication failed.');
  }

  return response.json();
}

export const loginAccount = (credentials) => postCredentials('login', credentials);
export const registerAccount = (credentials) => postCredentials('register', credentials);
