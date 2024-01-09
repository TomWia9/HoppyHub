import {
  HttpEvent,
  HttpHandlerFn,
  HttpHeaders,
  HttpInterceptorFn,
  HttpRequest
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, of } from 'rxjs';
import { take, exhaustMap } from 'rxjs/operators';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const authService: AuthService = inject(AuthService);

  return authService.user.pipe(
    take(1),
    exhaustMap(user => {
      if (!user) {
        return next(req);
      }

      if (
        user.tokenExpirationDate &&
        new Date() > new Date(user.tokenExpirationDate)
      ) {
        authService.logout();
        return of(null) as unknown as Observable<HttpEvent<unknown>>;
      }

      const modifiedReq = req.clone({
        headers: new HttpHeaders().set('Authorization', `Bearer ${user.token}`)
      });

      return next(modifiedReq);
    })
  );
};
