import { HttpInterceptorFn } from '@angular/common/http';
import { readStoredToken } from '../constants/auth-storage';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = readStoredToken();
  if (!token) {
    return next(req);
  }
  return next(
    req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    }),
  );
};
