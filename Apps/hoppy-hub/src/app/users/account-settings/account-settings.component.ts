import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { UsersService } from '../users.service';
import { Subscription } from 'rxjs';
import { AuthUser } from '../../auth/auth-user.model';
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

@Component({
  selector: 'app-account-settings',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './account-settings.component.html'
})
export class AccountSettingsComponent implements OnInit, OnDestroy {
  private authService: AuthService = inject(AuthService);
  private usersService: UsersService = inject(UsersService);
  private modalService: ModalService = inject(ModalService);
  private userSubscription!: Subscription;
  private authUserSubscription!: Subscription;

  loading: boolean = true;
  error: string = '';
  user: User | undefined;
  updateUsernameForm!: FormGroup;
  passwordForm!: FormGroup;

  ngOnInit(): void {
    this.authUserSubscription = this.authService.user.subscribe(
      (user: AuthUser | null) => {
        if (!user) {
          this.modalService.openModal(new ModalModel(ModalType.Login));
        } else {
          this.userSubscription = this.usersService
            .getUserById(user?.id as string)
            .subscribe({
              next: (user: User) => {
                this.user = user;
                this.updateUsernameForm = new FormGroup({
                  userName: new FormControl(this.user?.username, [
                    Validators.required,
                    Validators.maxLength(256)
                  ])
                });
                this.error = '';
                this.loading = false;
              },
              error: () => {
                this.error = 'An error occurred while loading the user';
                this.loading = false;
              }
            });
        }

        this.loading = false;
      }
    );
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
    console.log('updateUsername');
  }

  changePassword(): void {
    console.log('changePassword');
  }

  deleteAccount(): void {
    console.log('deleteAccount');
  }

  ngOnDestroy(): void {
    if (this.authUserSubscription) {
      this.authUserSubscription.unsubscribe();
    }
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }
}
