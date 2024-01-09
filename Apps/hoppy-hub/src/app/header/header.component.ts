import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ModalService, ModalType } from '../services/modal.service';
import { AuthService } from '../auth/auth.service';
import { Subscription } from 'rxjs';
import { User } from '../auth/user.model';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit, OnDestroy {
  private modalService = inject(ModalService);
  authService: AuthService = inject(AuthService);
  user: User | null | undefined;
  userSubscription!: Subscription;

  ngOnInit(): void {
    this.userSubscription = this.authService.user.subscribe(
      (user: User | null) => {
        this.user = user;
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
