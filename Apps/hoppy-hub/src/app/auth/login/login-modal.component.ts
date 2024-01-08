import { CommonModule } from '@angular/common';
import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  inject
} from '@angular/core';
import { Subscription } from 'rxjs';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { ModalService, ModalType } from '../../services/modal.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login-modal.component.html',
  styleUrl: './login-modal.component.css'
})
export class LoginModalComponent implements OnInit, OnDestroy {
  private modalService = inject(ModalService);
  private router: Router = inject(Router);

  @ViewChild('loginModal') myModalRef!: ElementRef;
  modalOpenedSubscription!: Subscription;
  loginForm!: FormGroup;
  errorMessage = '';

  ngOnInit(): void {
    this.modalOpenedSubscription = this.modalService.modalOpened.subscribe(
      (modalType: ModalType) => {
        this.showModal(modalType);
      }
    );
    this.loginForm = new FormGroup({
      email: new FormControl('', [
        Validators.email,
        Validators.required,
        Validators.maxLength(256)
      ]),
      password: new FormControl('', [Validators.required])
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      console.log('Sign in data:', this.loginForm.value);
      //authService.login();
      this.onFormReset();
      this.router.navigate(['/']);
    } else {
      console.log('The form is invalid');
    }
  }

  onSignUp() {
    this.onFormReset();
    this.modalService.openModal(ModalType.Register);
  }

  onFormReset() {
    this.errorMessage = '';
    this.loginForm.reset();
    if (this.myModalRef) {
      (this.myModalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  showModal(modalType: ModalType) {
    if (modalType === ModalType.Login && this.myModalRef) {
      (this.myModalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    this.modalOpenedSubscription.unsubscribe();
  }
}
