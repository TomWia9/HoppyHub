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
import { OpinionsService } from '../opinions.service';
import { ModalService } from '../../services/modal.service';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Subscription } from 'rxjs';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { UpsertOpinionCommand } from '../upsert-opinion-command.model';
import { Beer } from '../../beers/beer.model';
import {
  AlertService,
  AlertType
} from '../../shared-components/alert/alert.service';
import { Opinion } from '../opinion.model';
import { HttpErrorResponse } from '@angular/common/http';
import { ModalType } from '../../shared/model-type';
import { ModalModel } from '../../shared/modal-model';

@Component({
  selector: 'app-upsert-opinion-modal',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    ReactiveFormsModule
  ],
  templateUrl: './upsert-opinion-modal.component.html'
})
export class UpsertOpinionModalComponent implements OnInit, OnDestroy {
  @Output() opinionUpserted = new EventEmitter<void>();
  @ViewChild('upsertOpinionModal') modalRef!: ElementRef;
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  private modalService = inject(ModalService);
  private opinionsService = inject(OpinionsService);
  private alertService: AlertService = inject(AlertService);
  private modalOppenedSubscription!: Subscription;
  private addOpinionSubscription!: Subscription;

  beer!: Beer;
  existingOpinion: Opinion | null = null;

  opinionForm!: FormGroup;
  error = '';
  loading = true;
  showImage = true;
  imageUri: string | null = null;
  ratingValues: number[] = Array.from({ length: 10 }, (_, i) => i + 1);
  selectedImage: File | null = null;

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalModel: ModalModel) => {
        this.loading = true;

        if (modalModel.modalData['beer']) {
          this.beer = modalModel.modalData['beer'] as Beer;
        }
        this.existingOpinion = modalModel.modalData['opinion'] as Opinion;

        this.imageUri = `${this.existingOpinion?.imageUri}?timestamp=${new Date().getTime()}`;
        this.opinionForm = this.existingOpinion
          ? this.getExistingOpinionForm()
          : this.getOpinionForm();
        this.setShowImage();

        this.onShowModal(modalModel);
        this.loading = false;
      }
    );
  }

  onModalHide(): void {
    if (this.modalRef) {
      if (this.fileInput) {
        this.fileInput.nativeElement.value = '';
        this.selectedImage = null;
      }

      this.setShowImage();

      (this.modalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onSubmit(): void {
    this.loading = true;
    if (this.opinionForm.valid) {
      const upsertOpinionCommand = this.opinionForm
        .value as UpsertOpinionCommand;

      upsertOpinionCommand.beerId = this.beer.id;
      upsertOpinionCommand.image = this.selectedImage;

      if (this.existingOpinion) {
        upsertOpinionCommand.id = this.existingOpinion.id;
        upsertOpinionCommand.deleteImage =
          !this.selectedImage && !this.showImage ? true : false;
        this.opinionsService
          .UpdateOpinion(this.existingOpinion.id, upsertOpinionCommand)
          .subscribe({
            next: () => {
              this.opinionForm.reset();
              this.imageUri = `${this.existingOpinion?.imageUri}?timestamp=${new Date().getTime()}`;
              this.alertService.openAlert(AlertType.Success, 'Opinion updated');
              this.opinionUpserted.emit();
              this.loading = false;
            },
            error: error => {
              this.handleError(error);
            }
          });
      } else {
        this.opinionsService.CreateOpinion(upsertOpinionCommand).subscribe({
          next: () => {
            this.opinionForm.reset();
            this.alertService.openAlert(AlertType.Success, 'Opinion created');
            this.opinionUpserted.emit();
            this.loading = false;
          },
          error: error => {
            this.handleError(error);
          }
        });
      }
    }

    this.onModalHide();
    this.opinionForm.reset();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.selectedImage = input.files[0];
    }
  }

  onRemoveImage(): void {
    this.selectedImage = null;
    this.showImage = false;
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }

  private onShowModal(modalModel: ModalModel): void {
    if (modalModel.modalType === ModalType.UpsertOpinion && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  private getOpinionForm(): FormGroup {
    return new FormGroup({
      rating: new FormControl(null, Validators.required),
      comment: new FormControl('', {
        nonNullable: true,
        validators: [Validators.maxLength(1000)]
      })
    });
  }

  private getExistingOpinionForm(): FormGroup {
    return new FormGroup({
      rating: new FormControl(
        this.existingOpinion?.rating,
        Validators.required
      ),
      comment: new FormControl(this.existingOpinion?.comment, {
        nonNullable: true,
        validators: [Validators.maxLength(1000)]
      })
    });
  }

  private handleError(error: HttpErrorResponse) {
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

    this.loading = false;
  }

  private setShowImage(): void {
    this.showImage =
      this.existingOpinion?.imageUri &&
      this.existingOpinion?.imageUri.length > 0
        ? true
        : false;
  }

  ngOnDestroy(): void {
    if (this.modalOppenedSubscription) {
      this.modalOppenedSubscription.unsubscribe();
    }
    if (this.addOpinionSubscription) {
      this.addOpinionSubscription.unsubscribe();
    }
  }
}
