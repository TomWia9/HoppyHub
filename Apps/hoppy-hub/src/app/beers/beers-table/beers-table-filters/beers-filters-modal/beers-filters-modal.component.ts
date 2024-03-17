import { CommonModule } from '@angular/common';
import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  inject
} from '@angular/core';
import { ReactiveFormsModule, FormGroup } from '@angular/forms';
import { Subscription } from 'rxjs';
import { AlertService } from '../../../../alert/alert.service';
import { ModalService, ModalType } from '../../../../services/modal.service';
import { BeersService } from '../../../beers.service';

@Component({
  selector: 'app-beers-filters-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './beers-filters-modal.component.html',
  styleUrl: './beers-filters-modal.component.css'
})
export class BeersFiltersModalComponent implements OnInit, OnDestroy {
  private modalService = inject(ModalService);
  private beersService = inject(BeersService);
  private alertService = inject(AlertService);

  @ViewChild('beersFiltersModal') modalRef!: ElementRef;
  modalOppenedSubscription!: Subscription;
  beersFiltersForm!: FormGroup;

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalType: ModalType) => {
        this.showModal(modalType);
      }
    );
    this.beersFiltersForm = new FormGroup({});
  }

  onSubmit() {}

  onFormReset() {
    this.beersFiltersForm.reset();
    if (this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  showModal(modalType: ModalType) {
    if (modalType === ModalType.BeersFilters && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    this.modalOppenedSubscription.unsubscribe();
  }
}
