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
import { ModalService } from '../../services/modal.service';
import {
  AlertService,
  AlertType
} from '../../shared-components/alert/alert.service';
import { OpinionsService } from '../opinions.service';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { ModalModel } from '../../shared/modal-model';
import { ModalType } from '../../shared/model-type';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { Subject, takeUntil, tap } from 'rxjs';

@Component({
  selector: 'app-delete-opinion-modal',
  standalone: true,
  imports: [ErrorMessageComponent, LoadingSpinnerComponent],
  templateUrl: './delete-opinion-modal.component.html'
})
export class DeleteOpinionModalComponent implements OnInit, OnDestroy {
  @Output() opinionDeleted = new EventEmitter<void>();
  @ViewChild('deleteOpinionModal') modalRef!: ElementRef;

  private modalService = inject(ModalService);
  private opinionsService = inject(OpinionsService);
  private alertService: AlertService = inject(AlertService);
  private destroy$ = new Subject<void>();

  loading = true;
  opinionId!: string;
  error = '';

  ngOnInit(): void {
    this.modalService.modalOpened
      .pipe(
        takeUntil(this.destroy$),
        tap((modalModel: ModalModel) => {
          this.opinionId = modalModel.modalData['opinionId'] as string;
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
    this.opinionsService
      .DeleteOpinion(this.opinionId)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: () => {
            this.alertService.openAlert(AlertType.Success, 'Opinion deleted');
            this.opinionDeleted.emit();
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
        })
      )
      .subscribe();

    this.onModalHide();
  }

  private onShowModal(modalModel: ModalModel): void {
    if (modalModel.modalType === ModalType.DeleteOpinion && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
