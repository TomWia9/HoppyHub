import { CommonModule } from '@angular/common';
import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  inject
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Subscription } from 'rxjs';
import { ModalService, ModalType } from '../../services/modal.service';
import { Router } from '@angular/router';
import { CustomValidators } from '../../shared/custom-validators';
import { AuthService } from '../auth.service';
import { AlertService, AlertType } from '../../alert/alert.service';

@Component({
  selector: 'app-register-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register-modal.component.html',
  styleUrl: './register-modal.component.css'
})
export class RegisterModalComponent implements OnInit, OnDestroy {
  private modalService = inject(ModalService);
  private authService = inject(AuthService);
  private alertService = inject(AlertService);
  private router: Router = inject(Router);

  @ViewChild('registerModal') myModalRef!: ElementRef;
  modalOppenedSubscription!: Subscription;
  registerForm!: FormGroup;

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalType: ModalType) => {
        this.showModal(modalType);
      }
    );
    this.registerForm = new FormGroup({
      email: new FormControl('', [
        Validators.email,
        Validators.required,
        Validators.maxLength(256)
      ]),
      username: new FormControl('', [
        Validators.required,
        Validators.maxLength(256)
      ]),
      passwords: new FormGroup(
        {
          password: new FormControl('', [
            Validators.minLength(8),
            Validators.required,
            Validators.pattern(CustomValidators.passwordPattern)
          ]),
          confirmPassword: new FormControl('', [])
        },
        CustomValidators.passwordMatchValidator()
      )
    });
  }

  onFormReset() {
    this.registerForm.reset();
    if (this.myModalRef) {
      (this.myModalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onSubmit() {
    if (this.registerForm.valid) {
      console.log('Sign up data:', this.registerForm.value);
      this.authService
        .register(
          this.registerForm.value.email,
          this.registerForm.value.username,
          this.registerForm.value.passwords.password
        )
        .subscribe({
          next: () => {
            this.onFormReset();
            this.router.navigate(['/']);
          },
          error: error => {
            const errorMessage = this.getErrorMessage(error.error.errors);

            this.alertService.openAlert(AlertType.Error, errorMessage);
          }
        });
    }
  }

  onSignIn() {
    this.onFormReset();
    this.modalService.openModal(ModalType.Login);
  }

  showModal(modalType: ModalType) {
    if (modalType === ModalType.Register && this.myModalRef) {
      (this.myModalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  getErrorMessage(array: { [key: string]: string }[]): string {
    const firstObject = Object.values(array)[0];
    const errorMessage = Object.values(firstObject)[0];

    if (!errorMessage) {
      return 'Something went wrong';
    }

    return errorMessage;
  }

  ngOnDestroy(): void {
    this.modalOppenedSubscription.unsubscribe();
  }
}
