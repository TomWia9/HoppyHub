import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ModalService } from '../services/modal.service';
import { AuthService } from '../auth/auth.service';
import { Subscription } from 'rxjs';
import { RouterModule } from '@angular/router';
import { Roles } from '../auth/roles';
import { AuthUser } from '../auth/auth-user.model';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  AlertService,
  AlertType
} from '../shared-components/alert/alert.service';
import { faBars, faUser } from '@fortawesome/free-solid-svg-icons';
import { ModalModel } from '../shared/modal-model';
import { ModalType } from '../shared/model-type';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule, FontAwesomeModule],
  templateUrl: './header.component.html'
})
export class HeaderComponent implements OnInit, OnDestroy {
  private modalService = inject(ModalService);

  authService: AuthService = inject(AuthService);
  alertService: AlertService = inject(AlertService);

  user: AuthUser | null | undefined;
  userSubscription!: Subscription;
  adminAccess: boolean = false;
  faBars = faBars;
  faUser = faUser;

  ngOnInit(): void {
    this.userSubscription = this.authService.user.subscribe(
      (user: AuthUser | null) => {
        this.user = user;
        this.adminAccess = user?.role == Roles.Administrator;
      }
    );
  }

  openLoginModal(): void {
    this.modalService.openModal(new ModalModel(ModalType.Login));
  }

  openRegisterModal(): void {
    this.modalService.openModal(new ModalModel(ModalType.Register));
  }

  onLogout(): void {
    this.authService.logout();
    this.alertService.openAlert(AlertType.Info, 'Logged out successfully.');
  }

  ngOnDestroy(): void {
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }
}
