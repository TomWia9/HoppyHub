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
    console.log(this.opinionForm.value.rating);
    console.log(this.opinionForm.value.comment);
    console.log(this.opinionForm.value.image);

    // const opinion = new CreateOpinionCommand(

    // );

    // this.opinionsService.CreateOpinion();

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
