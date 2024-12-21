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
import { Subscription } from 'rxjs';
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
  private modalOppenedSubscription!: Subscription;
  private deleteBeerSubscription!: Subscription;

  loading = true;
  beerId!: string;
  error = '';

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalModel: ModalModel) => {
        this.loading = true;
        this.beerId = modalModel.modalData['beerId'] as string;
        this.onShowModal(modalModel);
        this.loading = false;
      }
    );
  }

  onModalHide(): void {
    if (this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onDelete(): void {
    this.deleteBeerSubscription = this.beersService
      .deleteBeer(this.beerId)
      .subscribe({
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
      });
    this.onModalHide();
  }

  private onShowModal(modalModel: ModalModel): void {
    if (modalModel.modalType === ModalType.DeleteBeer && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    if (this.modalOppenedSubscription) {
      this.modalOppenedSubscription.unsubscribe();
    }
    if (this.deleteBeerSubscription) {
      this.deleteBeerSubscription.unsubscribe();
    }
  }
}
