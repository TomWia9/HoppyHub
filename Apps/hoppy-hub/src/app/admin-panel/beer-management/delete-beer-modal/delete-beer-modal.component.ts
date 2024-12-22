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
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { finalize, Subject, takeUntil, tap } from 'rxjs';
import { ModalService } from '../../../services/modal.service';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { ModalModel } from '../../../shared/modal-model';
import { ModalType } from '../../../shared/model-type';
import { BeersService } from '../../../beers/beers.service';

@Component({
  selector: 'app-delete-beer-modal',
  standalone: true,
  imports: [LoadingSpinnerComponent, ErrorMessageComponent],
  templateUrl: './delete-beer-modal.component.html'
})
export class DeleteBeerModalComponent implements OnInit, OnDestroy {
  @Output() beerDeleted = new EventEmitter<void>();
  @ViewChild('deleteBeerModal') modalRef!: ElementRef;

  private modalService = inject(ModalService);
  private beersService = inject(BeersService);
  private alertService: AlertService = inject(AlertService);
  private destroy$ = new Subject<void>();

  loading = true;
  beerId!: string;
  error = '';

  ngOnInit(): void {
    this.modalService.modalOpened
      .pipe(
        takeUntil(this.destroy$),
        tap((modalModel: ModalModel) => {
          this.loading = true;
          this.beerId = modalModel.modalData['beerId'] as string;
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
    this.beersService
      .deleteBeer(this.beerId)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: () => {
            this.alertService.openAlert(AlertType.Success, 'Beer deleted');
            this.beerDeleted.emit();
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
    if (modalModel.modalType === ModalType.DeleteBeer && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
