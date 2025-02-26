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
import { BreweriesService } from '../../../breweries/breweries.service';
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
  selector: 'app-delete-brewery-modal',
  standalone: true,
  imports: [LoadingSpinnerComponent, ErrorMessageComponent],
  templateUrl: './delete-brewery-modal.component.html'
})
export class DeleteBreweryModalComponent implements OnInit, OnDestroy {
  @Output() breweryDeleted = new EventEmitter<void>();
  @ViewChild('deleteBreweryModal') modalRef!: ElementRef;

  private modalService = inject(ModalService);
  private breweriesService = inject(BreweriesService);
  private alertService: AlertService = inject(AlertService);
  private destroy$ = new Subject<void>();

  loading = true;
  brewreyId!: string;
  error = '';

  ngOnInit(): void {
    this.modalService.modalOpened
      .pipe(
        takeUntil(this.destroy$),
        tap((modalModel: ModalModel) => {
          this.loading = true;
          this.brewreyId = modalModel.modalData['breweryId'] as string;
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
    this.breweriesService
      .deleteBrewery(this.brewreyId)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: () => {
            this.alertService.openAlert(AlertType.Success, 'Brewery deleted');
            this.breweryDeleted.emit();
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
    if (modalModel.modalType === ModalType.DeleteBrewery && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
