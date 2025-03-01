import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { UsersService } from '../../../users/users.service';
import { ModalType } from '../../../shared/model-type';
import { UsersParams } from '../../../users/users-params';
import { User } from '../../../users/user.model';
import { HttpErrorResponse } from '@angular/common/http';
import {
  FormGroup,
  FormControl,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil, tap, map, switchMap } from 'rxjs';
import { ModalService } from '../../../services/modal.service';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { ModalModel } from '../../../shared/modal-model';
import { UpdateUserPasswordCommand } from '../../../users/commands/update-user-password-command.model';
import { UpdateUsernameCommand } from '../../../users/commands/update-username-command.model';
import { CustomValidators } from '../../../shared/custom-validators';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { DeleteUserModalComponent } from '../delete-user-modal/delete-user-modal.component';

@Component({
  selector: 'app-edit-user',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    ReactiveFormsModule,
    DeleteUserModalComponent
  ],
  templateUrl: './edit-user.component.html'
})
export class EditUserComponent implements OnInit, OnDestroy {
  private route: ActivatedRoute = inject(ActivatedRoute);
  private router: Router = inject(Router);
  private usersService: UsersService = inject(UsersService);
  private alertService: AlertService = inject(AlertService);
  private modalService: ModalService = inject(ModalService);
  private destroy$ = new Subject<void>();

  user!: User;
  error = '';
  loading = true;
  usernameForm!: FormGroup;
  passwordForm!: FormGroup;

  ngOnInit(): void {
    this.getUser();
  }

  onUserDelete(): void {
    this.modalService.openModal(
      new ModalModel(ModalType.DeleteUser, {
        userId: this.user.id
      })
    );
  }

  editUsername(): void {
    const updateUsernameCommand = this.usernameForm
      .value as UpdateUsernameCommand;
    updateUsernameCommand.userId = this.user!.id;
    this.usersService
      .UpdateUsername(this.user!.id, updateUsernameCommand)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: () => {
            this.usernameForm.reset();
            this.alertService.openAlert(AlertType.Success, 'Username changed');
            this.getUser();
            this.userChanged();
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

  editPassword(): void {
    const updateUserPasswordCommand = this.passwordForm
      .value as UpdateUserPasswordCommand;
    updateUserPasswordCommand.userId = this.user!.id;
    this.usersService
      .UpdateUserPassword(this.user!.id, updateUserPasswordCommand)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: () => {
            this.alertService.openAlert(AlertType.Success, 'Password changed.');
            this.passwordForm.reset();
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

  userChanged(): void {
    this.usersService.paramsChanged.next(
      new UsersParams({
        pageSize: 15,
        pageNumber: 1,
        sortBy: 'Created',
        sortDirection: 1
      })
    );
  }

  userDeleted(): void {
    this.userChanged();
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  private getUser(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroy$),
        map(params => params.get('id')),
        switchMap(userId =>
          this.usersService.getUserById(userId as string).pipe(
            tap({
              next: (user: User) => {
                this.user = user;
                this.initForms(user);
                this.error = '';
                this.loading = false;
              },
              error: () => {
                this.error = 'An error occurred while loading the user';
                this.loading = false;
              }
            })
          )
        )
      )
      .subscribe();
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = null;

    if (error.error?.errors) {
      const firstKey = Object.keys(error.error.errors)[0] ?? null;
      const firstValueArray = error.error.errors[firstKey] as string[];
      errorMessage = firstValueArray[0];
    } else if (error.error?.detail) {
      errorMessage = error.error.detail;
    }

    if (!errorMessage) {
      this.alertService.openAlert(AlertType.Error, 'Something went wrong');
    } else {
      this.alertService.openAlert(AlertType.Error, errorMessage);
    }

    this.loading = false;
  }

  private initForms(user: User): void {
    this.usernameForm = new FormGroup({
      userName: new FormControl(user?.username, [
        Validators.required,
        Validators.maxLength(256)
      ])
    });
    this.passwordForm = new FormGroup({
      newPassword: new FormControl('', [
        Validators.minLength(8),
        Validators.required,
        Validators.pattern(CustomValidators.passwordPattern)
      ])
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
