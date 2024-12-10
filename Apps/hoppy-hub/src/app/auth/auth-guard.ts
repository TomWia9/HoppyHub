import { inject } from '@angular/core';
import { Router, ActivatedRouteSnapshot, CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';
import { take, map } from 'rxjs';
import { ModalService } from '../services/modal.service';
import { ModalModel } from '../shared/modal-model';
import { ModalType } from '../shared/model-type';

export const authGuard: CanActivateFn = (next: ActivatedRouteSnapshot) => {
  const authService: AuthService = inject(AuthService);
  const router: Router = inject(Router);
  const modalService: ModalService = inject(ModalService);

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
          modalService.openModal(new ModalModel(ModalType.Login));
          return router.createUrlTree(['/']);
        }
      }
      modalService.openModal(new ModalModel(ModalType.Login));
      return router.createUrlTree(['/']);
    })
  );
};
