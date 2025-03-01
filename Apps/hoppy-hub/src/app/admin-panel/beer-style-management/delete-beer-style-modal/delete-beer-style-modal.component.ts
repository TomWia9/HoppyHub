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
import { BeerStylesService } from '../../../beer-styles/beer-styles.service';
import { Subject, takeUntil, tap, finalize } from 'rxjs';
import { ModalService } from '../../../services/modal.service';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { ModalModel } from '../../../shared/modal-model';
import { ModalType } from '../../../shared/model-type';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';

@Component({
  selector: 'app-delete-beer-style-modal',
  standalone: true,
  imports: [LoadingSpinnerComponent, ErrorMessageComponent],
  templateUrl: './delete-beer-style-modal.component.html'
})
export class DeleteBeerStyleModalComponent implements OnInit, OnDestroy {
  @Output() beerStyleDeleted = new EventEmitter<void>();
  @ViewChild('deleteBeerStyleModal') modalRef!: ElementRef;

  private modalService = inject(ModalService);
  private beerStylesService = inject(BeerStylesService);
  private alertService: AlertService = inject(AlertService);
  private destroy$ = new Subject<void>();

  loading = true;
  beerStyleId!: string;
  error = '';

  ngOnInit(): void {
    this.modalService.modalOpened
      .pipe(
        takeUntil(this.destroy$),
        tap((modalModel: ModalModel) => {
          this.loading = true;
          this.beerStyleId = modalModel.modalData['beerStyleId'] as string;
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
    this.beerStylesService
      .deleteBeerStyle(this.beerStyleId)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: () => {
            this.alertService.openAlert(
              AlertType.Success,
              'Beer style deleted'
            );
            this.beerStyleDeleted.emit();
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
    if (modalModel.modalType === ModalType.DeleteBeerStyle && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
