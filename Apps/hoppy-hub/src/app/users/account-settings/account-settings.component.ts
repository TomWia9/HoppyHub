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

@Component({
  selector: 'app-account-settings',
  standalone: true,
  imports: [LoadingSpinnerComponent, ErrorMessageComponent],
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
