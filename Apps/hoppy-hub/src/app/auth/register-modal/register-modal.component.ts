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
import { ModalService } from '../../services/modal.service';
import { CustomValidators } from '../../shared/custom-validators';
import { AuthService } from '../auth.service';
import {
  AlertService,
  AlertType
} from '../../shared-components/alert/alert.service';
import { ModalModel } from '../../shared/modal-model';
import { ModalType } from '../../shared/model-type';

@Component({
  selector: 'app-register-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register-modal.component.html'
})
export class RegisterModalComponent implements OnInit, OnDestroy {
  @ViewChild('registerModal') myModalRef!: ElementRef;

  private modalService = inject(ModalService);
  private authService = inject(AuthService);
  private alertService = inject(AlertService);

  modalOppenedSubscription!: Subscription;
  registerForm!: FormGroup;

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalModel: ModalModel) => {
        this.showModal(modalModel);
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

  onFormReset(): void {
    this.registerForm.reset();
    if (this.myModalRef) {
      (this.myModalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.authService
        .register(
          this.registerForm.value.email,
          this.registerForm.value.username,
          this.registerForm.value.passwords.password
        )
        .subscribe({
          next: () => {
            this.onFormReset();
            this.alertService.openAlert(
              AlertType.Success,
              'Successfully registered'
            );
          },
          error: error => {
            let errorMessage = 'Something went wrong';

            if (error.error && error.error.errors) {
              errorMessage = this.getErrorMessage(error.error.errors);
            }
            console.log(errorMessage);

            this.alertService.openAlert(AlertType.Error, errorMessage);
          }
        });
    }
  }

  onSignIn(): void {
    this.onFormReset();
    this.modalService.openModal(new ModalModel(ModalType.Login));
  }

  private showModal(modalModel: ModalModel): void {
    if (modalModel.modalType === ModalType.Register && this.myModalRef) {
      (this.myModalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  private getErrorMessage(array: { [key: string]: string }[]): string {
    if (array.length === 0) {
      return 'Something went wrong';
    }
    const errorMessage = Object.values(array)[0] as unknown as string;

    if (!errorMessage) {
      return 'Something went wrong';
    }

    return errorMessage;
  }

  ngOnDestroy(): void {
    this.modalOppenedSubscription.unsubscribe();
  }
}
