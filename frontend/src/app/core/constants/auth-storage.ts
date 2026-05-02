export const AUTH_TOKEN_KEY = 'tm_token';

export function readStoredToken(): string | null {
  return localStorage.getItem(AUTH_TOKEN_KEY);
}
