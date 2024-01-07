import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginModalService } from '../auth/login/login-modal.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  private loginModalService = inject(LoginModalService);

  isUserLoggedIn: boolean = false;

  openLoginModal() {
    this.loginModalService.openModal();
  }
}
