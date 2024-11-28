import {
  Component,
  ElementRef,
  inject,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { UsersService } from '../../users.service';
import { Subscription } from 'rxjs';
import { ModalService } from '../../../services/modal.service';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { ModalModel } from '../../../shared/modal-model';
import { ModalType } from '../../../shared/model-type';
import { AuthService } from '../../../auth/auth.service';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { DeleteUserCommand } from '../../commands/delete-user-command.model';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-delete-account-modal',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './delete-account-modal.component.html'
})
export class DeleteAccountModalComponent implements OnInit, OnDestroy {
  @ViewChild('deleteAccountModal') modalRef!: ElementRef;

  private modalService = inject(ModalService);
  private usersService = inject(UsersService);
  private alertService: AlertService = inject(AlertService);
  private authService: AuthService = inject(AuthService);
  private modalOppenedSubscription!: Subscription;
  private deleteAccountSubscription!: Subscription;

  passwordForm!: FormGroup;
  loading = true;
  userId!: string;
  error = '';

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalModel: ModalModel) => {
        this.loading = true;
        this.userId = modalModel.modalData['userId'] as string;
        this.onShowModal(modalModel);
        this.loading = false;
      }
    );

    this.passwordForm = new FormGroup({
      password: new FormControl('', [Validators.required])
    });
  }

  onModalHide(): void {
    if (this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onDelete(): void {
    const deleteUserCommand = this.passwordForm.value as DeleteUserCommand;
    deleteUserCommand.userId = this.userId;
    this.usersService.DeleteAccount(this.userId, deleteUserCommand).subscribe({
      next: () => {
        this.alertService.openAlert(AlertType.Success, 'Account deleted');
        this.authService.logout();
        this.passwordForm.reset();
      },
      error: error => {
        this.handleError(error);
        this.passwordForm.reset();
      }
    });
    this.onModalHide();
  }

  private onShowModal(modalModel: ModalModel): void {
    if (modalModel.modalType === ModalType.DeleteOpinion && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
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

    this.loading = false;
  }

  ngOnDestroy(): void {
    if (this.modalOppenedSubscription) {
      this.modalOppenedSubscription.unsubscribe();
    }
    if (this.deleteAccountSubscription) {
      this.deleteAccountSubscription.unsubscribe();
    }
  }
}
