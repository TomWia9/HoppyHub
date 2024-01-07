import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ModalService, ModalType } from '../services/modal.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  private modalService = inject(ModalService);

  isUserLoggedIn: boolean = false;

  openLoginModal() {
    this.modalService.openModal(ModalType.Login);
  }
  openRegisterModal() {
    this.modalService.openModal(ModalType.Register);
  }
}
