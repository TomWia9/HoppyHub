import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ModalService, ModalType } from '../services/modal.service';
import { AuthService } from '../auth/auth.service';
import { Subscription } from 'rxjs';
import { RouterModule } from '@angular/router';
import { Roles } from '../auth/roles';
import { AuthUser } from '../auth/auth-user.model';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html'
})
export class HeaderComponent implements OnInit, OnDestroy {
  private modalService = inject(ModalService);
  authService: AuthService = inject(AuthService);
  user: AuthUser | null | undefined;
  userSubscription!: Subscription;
  adminAccess: boolean = false;

  ngOnInit(): void {
    this.userSubscription = this.authService.user.subscribe(
      (user: AuthUser | null) => {
        this.user = user;
        this.adminAccess = user?.role == Roles.Administrator;
      }
    );
  }

  isUserLoggedIn: boolean = false;

  openLoginModal() {
    this.modalService.openModal(ModalType.Login);
  }

  openRegisterModal() {
    this.modalService.openModal(ModalType.Register);
  }

  onLogout() {
    this.authService.logout();
  }

  ngOnDestroy(): void {
    this.userSubscription.unsubscribe();
  }
}
