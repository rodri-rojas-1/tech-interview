import { HttpHeaders } from '@angular/common/http';
import { readStoredToken } from '../constants/auth-storage';

/** Extra options so authenticated calls carry Bearer even if DevTools omits it from the log. */
export function bearerAuthOptions(): { headers: HttpHeaders } | undefined {
  const token = readStoredToken();
  if (!token) {
    return undefined;
  }
  return {
    headers: new HttpHeaders({
      Authorization: `Bearer ${token}`,
    }),
  };
}
