import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { UsersService } from '../users.service';
import { User } from '../user.model';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { ModalService } from '../../services/modal.service';
import { ModalModel } from '../../shared/modal-model';
import { ModalType } from '../../shared/model-type';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { CustomValidators } from '../../shared/custom-validators';
import { CommonModule } from '@angular/common';
import { UpdateUsernameCommand } from '../commands/update-username-command.model';
import {
  AlertService,
  AlertType
} from '../../shared-components/alert/alert.service';
import { HttpErrorResponse } from '@angular/common/http';
import { UpdateUserPasswordCommand } from '../commands/update-user-password-command.model';
import { DeleteAccountModalComponent } from './delete-account-modal/delete-account-modal.component';
import { Subject, switchMap, takeUntil, tap } from 'rxjs';

@Component({
  selector: 'app-account-settings',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    ReactiveFormsModule,
    CommonModule,
    DeleteAccountModalComponent
  ],
  templateUrl: './account-settings.component.html'
})
export class AccountSettingsComponent implements OnInit, OnDestroy {
  private authService: AuthService = inject(AuthService);
  private usersService: UsersService = inject(UsersService);
  private modalService: ModalService = inject(ModalService);
  private alertService: AlertService = inject(AlertService);
  private destroy$ = new Subject<void>();

  loading: boolean = true;
  error: string = '';
  user: User | undefined;
  updateUsernameForm!: FormGroup;
  passwordForm!: FormGroup;

  ngOnInit(): void {
    this.getUser();

    this.updateUsernameForm = new FormGroup({
      userName: new FormControl(this.user?.username, [
        Validators.required,
        Validators.maxLength(256)
      ])
    });
    this.passwordForm = new FormGroup({
      currentPassword: new FormControl('', [Validators.required]),
      newPassword: new FormControl('', [
        Validators.minLength(8),
        Validators.required,
        Validators.pattern(CustomValidators.passwordPattern)
      ])
    });
  }

  updateUsername(): void {
    const updateUsernameCommand = this.updateUsernameForm
      .value as UpdateUsernameCommand;
    updateUsernameCommand.userId = this.user!.id;
    this.usersService
      .UpdateUsername(this.user!.id, updateUsernameCommand)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: () => {
            this.updateUsernameForm.reset();
            this.alertService.openAlert(AlertType.Success, 'Username changed');
            this.getUser();
          },
          error: error => {
            this.handleError(error);
          },
          complete: () => {
            this.loading = false;
          }
        })
      )
      .subscribe();
  }

  changePassword(): void {
    const updateUserPasswordCommand = this.passwordForm
      .value as UpdateUserPasswordCommand;
    updateUserPasswordCommand.userId = this.user!.id;
    this.usersService
      .UpdateUserPassword(this.user!.id, updateUserPasswordCommand)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: () => {
            this.alertService.openAlert(
              AlertType.Success,
              'The password has been changed, please log in again.'
            );
            this.passwordForm.reset();
            this.authService.logout();
          },
          error: error => {
            this.handleError(error);
          },
          complete: () => {
            this.loading = false;
          }
        })
      )
      .subscribe();
  }

  deleteAccount(): void {
    if (this.user) {
      this.modalService.openModal(
        new ModalModel(ModalType.DeleteOpinion, {
          userId: this.user?.id
        })
      );
    }
  }

  private getUser(): void {
    this.authService.user
      .pipe(
        takeUntil(this.destroy$),
        switchMap(user => {
          if (!user) {
            this.modalService.openModal(new ModalModel(ModalType.Login));
            return '';
          } else {
            return this.usersService.getUserById(user?.id as string).pipe(
              tap({
                next: user => {
                  this.user = user;
                  this.updateUsernameForm.controls['userName'].setValue(
                    user?.username
                  );
                  this.error = '';
                },
                error: () => {
                  this.error = 'An error occurred while loading the user';
                },
                complete: () => {
                  this.loading = false;
                }
              })
            );
          }
        })
      )
      .subscribe();
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = null;

    if (error.error && error.error.errors) {
      const firstKey = Object.keys(error.error?.errors)[0] ?? null;
      const firstValueArray = error.error?.errors[firstKey] as string[];
      errorMessage = firstValueArray[0];
    } else if (error.error && error.error.detail) {
      errorMessage = error.error.detail;
    }

    if (!errorMessage) {
      this.alertService.openAlert(AlertType.Error, 'Something went wrong');
    } else {
      this.alertService.openAlert(AlertType.Error, errorMessage);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
