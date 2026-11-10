const BASE_URL = `${import.meta.env.VITE_API_URL || 'http://localhost:5129'}/api/users`;

const authenticatedFetch = async (url, token, options = {}) => {
  const response = await fetch(url, {
    ...options,
    headers: {
      ...options.headers,
      Authorization: `Bearer ${token}`,
    },
  });
  if (response.status === 401) window.dispatchEvent(new Event('auth:expired'));
  return response;
};

const responseError = async (response, fallback) => {
  const body = await response.text();
  if (!body) return new Error(fallback);

  try {
    const errorData = JSON.parse(body);
    return new Error(errorData.title || errorData.message || fallback);
  } catch {
    return new Error(fallback);
  }
};

export const fetchUsers = async (token) => {
  const response = await authenticatedFetch(BASE_URL, token);
  if (!response.ok) throw await responseError(response, 'Failed to fetch users');
  return response.json();
};

export const createUser = async (userData, token) => {
  const response = await authenticatedFetch(BASE_URL, token, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(userData),
  });
  if (!response.ok) throw await responseError(response, 'Failed to create user');
  return response.json();
};