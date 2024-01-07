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
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

@Component({
  selector: 'app-login-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login-modal.component.html',
  styleUrl: './login-modal.component.css'
})
export class LoginModalComponent implements OnInit, OnDestroy {
  private loginModalService = inject(LoginModalService);
  @ViewChild('loginModal') myModalRef!: ElementRef;
  loginModalOppenedSubscription!: Subscription;
  loginForm!: FormGroup;
  errorMessage = '';

  ngOnInit(): void {
    this.loginModalOppenedSubscription =
      this.loginModalService.modalOpened.subscribe(() => {
        this.showModal();
      });
    this.loginForm = new FormGroup({
      email: new FormControl('', [
        Validators.email,
        Validators.required,
        Validators.maxLength(256)
      ]),
      password: new FormControl('', [
        Validators.minLength(8),
        Validators.maxLength(64),
        Validators.required
      ])
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      console.log('Sign in data:', this.loginForm.value);
    } else {
      console.log('The form is invalid');
    }
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
