import {
  Component,
  ElementRef,
  inject,
  Input,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { OpinionsService } from '../opinions.service';
import { ModalService, ModalType } from '../../services/modal.service';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Subscription } from 'rxjs';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { CreateOpinionCommand } from '../create-opinion-command.model';
import { Beer } from '../../beers/beer.model';
import {
  AlertService,
  AlertType
} from '../../shared-components/alert/alert.service';

@Component({
  selector: 'app-add-opinion-modal',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    ReactiveFormsModule
  ],
  templateUrl: './add-opinion-modal.component.html'
})
export class AddOpinionModalComponent implements OnInit, OnDestroy {
  @Input({ required: true }) beer!: Beer;

  private modalService = inject(ModalService);
  private opinionsService = inject(OpinionsService);
  private alertService: AlertService = inject(AlertService);

  @ViewChild('addOpinionModal') modalRef!: ElementRef;
  modalOppenedSubscription!: Subscription;
  opinionForm!: FormGroup;
  error = '';
  loading = false;
  addOpinionSubscription!: Subscription;
  ratingValues: number[] = Array.from({ length: 10 }, (_, i) => i + 1);

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalType: ModalType) => {
        this.onShowModal(modalType);
      }
    );

    this.opinionForm = this.getOpinionForm();
  }

  onModalHide() {
    if (this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onShowModal(modalType: ModalType) {
    if (modalType === ModalType.AddOpinion && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  onSubmit() {
    if (this.opinionForm.valid) {
      const createOpinionCommand = this.opinionForm
        .value as CreateOpinionCommand;
      createOpinionCommand.beerId = this.beer.id;

      this.opinionsService.CreateOpinion(createOpinionCommand).subscribe({
        next: () => {
          this.opinionForm.reset();
          this.alertService.openAlert(AlertType.Success, 'Opinion created');
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
    }

    this.onModalHide();
    this.opinionForm.reset();
  }

  private getOpinionForm(): FormGroup {
    return new FormGroup({
      rating: new FormControl(null, Validators.required),
      comment: new FormControl('', Validators.maxLength(1000)),
      image: new FormControl(null)
    });
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
