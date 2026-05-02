import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { readStoredToken } from '../constants/auth-storage';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = readStoredToken();
  if (!token) {
    return next(req);
  }
  let headers = req.headers.set('Authorization', `Bearer ${token}`);
  // DevTools often omit Authorization from the Network panel; this confirms the interceptor ran.
  if (!environment.production) {
    headers = headers.set('X-Debug-Auth', 'attached');
  }
  return next(req.clone({ headers }));
};
