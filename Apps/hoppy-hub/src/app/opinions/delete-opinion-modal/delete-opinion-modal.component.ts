import {
  Component,
  ElementRef,
  EventEmitter,
  inject,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import { ModalService, ModalType } from '../../services/modal.service';
import {
  AlertService,
  AlertType
} from '../../shared-components/alert/alert.service';
import { OpinionsService } from '../opinions.service';
import { Subscription } from 'rxjs';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';

@Component({
  selector: 'app-delete-opinion-modal',
  standalone: true,
  imports: [ErrorMessageComponent],
  templateUrl: './delete-opinion-modal.component.html'
})
export class DeleteOpinionModalComponent implements OnInit, OnDestroy {
  @Input({ required: true }) opinionId!: string;
  @Output() opinionDeleted = new EventEmitter<number>();
  @ViewChild('deleteOpinionModal') modalRef!: ElementRef;

  private modalService = inject(ModalService);
  private opinionsService = inject(OpinionsService);
  private alertService: AlertService = inject(AlertService);

  modalOppenedSubscription!: Subscription;
  deleteOpinionSubscription!: Subscription;
  error = '';

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalType: ModalType) => {
        this.onShowModal(modalType);
      }
    );
  }

  onModalHide(): void {
    if (this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onDelete(): void {
    this.opinionsService.DeleteOpinion(this.opinionId).subscribe({
      next: () => {
        this.alertService.openAlert(AlertType.Success, 'Opinion deleted');
        this.opinionDeleted.emit(-1);
      },
      error: error => {
        let errorMessage = null;

        if (error.error) {
          const firstKey = Object.keys(error.error?.errors)[0] ?? null;
          const firstValueArray = error.error?.errors[firstKey] as string[];
          errorMessage = firstValueArray[0];
        }

        if (!errorMessage) {
          this.alertService.openAlert(AlertType.Error, 'Something went wrong');
        } else {
          this.alertService.openAlert(AlertType.Error, errorMessage);
        }
      }
    });
  }

  private onShowModal(modalType: ModalType): void {
    if (modalType === ModalType.DeleteOpinion && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    if (this.modalOppenedSubscription) {
      this.modalOppenedSubscription.unsubscribe();
    }
    if (this.deleteOpinionSubscription) {
      this.deleteOpinionSubscription.unsubscribe();
    }
  }
}
