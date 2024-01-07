import { CommonModule } from '@angular/common';
import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  inject
} from '@angular/core';
import { LoginModalService } from './login-modal.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-login-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './login-modal.component.html',
  styleUrl: './login-modal.component.css'
})
export class LoginModalComponent implements OnInit, OnDestroy {
  private loginModalService = inject(LoginModalService);
  @ViewChild('loginModal') myModalRef!: ElementRef;
  loginModalOppenedSubscription!: Subscription;

  ngOnInit(): void {
    this.loginModalOppenedSubscription =
      this.loginModalService.modalOpened.subscribe(() => {
        this.showModal();
      });
  }

  showModal() {
    if (this.myModalRef) {
      (this.myModalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    this.loginModalOppenedSubscription.unsubscribe();
  }
}
