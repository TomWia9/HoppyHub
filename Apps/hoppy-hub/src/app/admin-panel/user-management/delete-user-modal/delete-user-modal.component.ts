import {
  Component,
  ElementRef,
  EventEmitter,
  inject,
  OnDestroy,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import { UsersService } from '../../../users/users.service';
import { Subject, takeUntil, tap, finalize } from 'rxjs';
import { ModalService } from '../../../services/modal.service';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { ModalModel } from '../../../shared/modal-model';
import { ModalType } from '../../../shared/model-type';
import { DeleteUserCommand } from '../../../users/commands/delete-user-command.model';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';

@Component({
  selector: 'app-delete-user-modal',
  standalone: true,
  imports: [LoadingSpinnerComponent, ErrorMessageComponent],
  templateUrl: './delete-user-modal.component.html'
})
export class DeleteUserModalComponent implements OnInit, OnDestroy {
  @Output() userDeleted = new EventEmitter<void>();
  @ViewChild('deleteUserModal') modalRef!: ElementRef;

  private modalService = inject(ModalService);
  private usersService = inject(UsersService);
  private alertService: AlertService = inject(AlertService);
  private destroy$ = new Subject<void>();

  loading = true;
  userId!: string;
  error = '';

  ngOnInit(): void {
    this.modalService.modalOpened
      .pipe(
        takeUntil(this.destroy$),
        tap((modalModel: ModalModel) => {
          this.loading = true;
          this.userId = modalModel.modalData['userId'] as string;
          this.onShowModal(modalModel);
          this.loading = false;
        })
      )
      .subscribe();
  }

  onModalHide(): void {
    if (this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onDelete(): void {
    const deleteUserCommand = new DeleteUserCommand(this.userId, undefined);
    this.usersService
      .DeleteAccount(this.userId, deleteUserCommand)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: () => {
            this.alertService.openAlert(AlertType.Success, 'User deleted');
            this.userDeleted.emit();
          },
          error: error => {
            let errorMessage = null;

            if (error.error) {
              const firstKey = Object.keys(error.error?.errors)[0] ?? null;
              const firstValueArray = error.error?.errors[firstKey] as string[];
              errorMessage = firstValueArray[0];
            }

            if (!errorMessage) {
              this.alertService.openAlert(
                AlertType.Error,
                'Something went wrong'
              );
            } else {
              this.alertService.openAlert(AlertType.Error, errorMessage);
            }
          }
        }),
        finalize(() => this.onModalHide())
      )
      .subscribe();
    this.onModalHide();
  }

  private onShowModal(modalModel: ModalModel): void {
    if (modalModel.modalType === ModalType.DeleteUser && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
