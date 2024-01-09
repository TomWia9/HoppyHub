import { inject } from '@angular/core';
import {
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  CanActivateFn
} from '@angular/router';
import { AuthService } from './auth.service';
import { take, map } from 'rxjs';

export const authGuard: CanActivateFn = (
  next: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const authService: AuthService = inject(AuthService);
  const router: Router = inject(Router);

  return authService.user.pipe(
    take(1),
    map(user => {
      const isAuthenticated = !!user;
      if (isAuthenticated) {
        if (
          (next.data['roles'] && user.role.includes(next.data['roles'][0])) ||
          !next.data['roles']
        ) {
          return true;
        } else {
          return router.createUrlTree(['/'], {
            queryParams: { returnUrl: state.url }
          });
        }
      }
      return router.createUrlTree(['/'], {
        queryParams: { returnUrl: state.url }
      });
    })
  );
};
