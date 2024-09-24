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
import {
  AlertService,
  AlertType
} from '../../shared-components/alert/alert.service';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login-modal.component.html'
})
export class LoginModalComponent implements OnInit, OnDestroy {
  @ViewChild('loginModal') myModalRef!: ElementRef;

  private modalService = inject(ModalService);
  private authService = inject(AuthService);
  private alertService = inject(AlertService);

  modalOpenedSubscription!: Subscription;
  loginForm!: FormGroup;

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

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.authService
        .login(this.loginForm.value.email, this.loginForm.value.password)
        .subscribe({
          next: () => {
            this.onFormReset();
            this.alertService.openAlert(
              AlertType.Success,
              'Logged in successfully'
            );
          },
          error: error => {
            const errorMessage = error.error.errors[0];

            if (!errorMessage) {
              this.alertService.openAlert(
                AlertType.Error,
                'Something went wrong'
              );
            } else {
              this.alertService.openAlert(AlertType.Error, errorMessage);
            }
          }
        });
    }
  }

  onSignUp(): void {
    this.onFormReset();
    this.modalService.openModal(ModalType.Register);
  }

  onFormReset(): void {
    this.loginForm.reset();
    if (this.myModalRef) {
      (this.myModalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  private showModal(modalType: ModalType): void {
    if (modalType === ModalType.Login && this.myModalRef) {
      (this.myModalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    if (this.modalOpenedSubscription) {
      this.modalOpenedSubscription.unsubscribe();
    }
  }
}
