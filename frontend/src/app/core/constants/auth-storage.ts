export const AUTH_TOKEN_KEY = 'tm_token';

export function readStoredToken(): string | null {
  const raw = localStorage.getItem(AUTH_TOKEN_KEY);
  if (raw == null) {
    return null;
  }
  const t = raw.trim();
  return t.length ? t : null;
}
