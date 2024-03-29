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
  private modalService = inject(ModalService);
  private authService = inject(AuthService);
  private alertService = inject(AlertService);
  private router: Router = inject(Router);

  @ViewChild('loginModal') myModalRef!: ElementRef;
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

  onSubmit() {
    if (this.loginForm.valid) {
      this.authService
        .login(this.loginForm.value.email, this.loginForm.value.password)
        .subscribe({
          next: () => {
            this.onFormReset();
            this.router.navigate(['/']);
          },
          error: error => {
            const errorMessage = error.error.errors[0];

            if (!errorMessage) {
              this.alertService.openAlert(
                AlertType.Error,
                'Something went wrong'
              );
            }

            this.alertService.openAlert(AlertType.Error, errorMessage);
          }
        });
    }
  }

  onSignUp() {
    this.onFormReset();
    this.modalService.openModal(ModalType.Register);
  }

  onFormReset() {
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
