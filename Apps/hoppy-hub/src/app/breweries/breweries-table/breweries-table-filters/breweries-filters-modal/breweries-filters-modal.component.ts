import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  inject
} from '@angular/core';
import {
  FormGroup,
  FormControl,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';
import { Subscription } from 'rxjs';
import { ModalService } from '../../../../services/modal.service';
import { CustomValidators } from '../../../../shared/custom-validators';
import { BreweriesParams } from '../../../breweries-params';
import { BreweriesService } from '../../../breweries.service';
import { Brewery } from '../../../brewery.model';
import { ErrorMessageComponent } from '../../../../shared-components/error-message/error-message.component';
import { CommonModule } from '@angular/common';
import { ModalModel } from '../../../../shared/modal-model';
import { ModalType } from '../../../../shared/model-type';

@Component({
  selector: 'app-breweries-filters-modal',
  standalone: true,
  templateUrl: './breweries-filters-modal.component.html',
  imports: [ErrorMessageComponent, ReactiveFormsModule, CommonModule]
})
export class BreweriesFiltersModalComponent implements OnInit, OnDestroy {
  @ViewChild('breweriesFiltersModal') modalRef!: ElementRef;

  private modalService = inject(ModalService);
  private breweriesService = inject(BreweriesService);

  breweriesFiltersForm!: FormGroup;
  breweries: Brewery[] = [];
  error = '';
  modalOppenedSubscription!: Subscription;
  sortOptions = BreweriesParams.sortOptions;
  currentYear = new Date().getFullYear();

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalModel: ModalModel) => {
        this.onShowModal(modalModel);
      }
    );

    this.breweriesFiltersForm = this.getBreweriesFiltersForm();
  }

  onSubmit(): void {
    const breweriesParams = new BreweriesParams({
      pageSize: 25,
      pageNumber: 1,
      sortBy: this.sortOptions[this.breweriesFiltersForm.value.sortBy].value,
      sortDirection:
        this.sortOptions[this.breweriesFiltersForm.value.sortBy].direction,
      name: this.breweriesFiltersForm.value.name,
      country: this.breweriesFiltersForm.value.country,
      minFoundationYear:
        this.breweriesFiltersForm.value.foundationYears.minFoundationYear,
      maxFoundationYear:
        this.breweriesFiltersForm.value.foundationYears.maxFoundationYear
    });

    this.breweriesService.paramsChanged.next(breweriesParams);

    this.onModalHide();
  }

  onModalHide(): void {
    if (this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onShowModal(modalModel: ModalModel): void {
    if (modalModel.modalType === ModalType.BreweriesFilters && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  onClearFilters(): void {
    this.breweriesFiltersForm.reset();
  }

  ngOnDestroy(): void {
    this.modalOppenedSubscription.unsubscribe();
  }

  private getBreweriesFiltersForm(): FormGroup {
    return new FormGroup({
      name: new FormControl('', [Validators.maxLength(500)]),
      country: new FormControl('', [Validators.maxLength(50)]),
      foundationYears: new FormGroup(
        {
          minFoundationYear: new FormControl('', [
            Validators.min(0),
            Validators.max(this.currentYear)
          ]),
          maxFoundationYear: new FormControl('', [
            Validators.min(0),
            Validators.max(this.currentYear)
          ])
        },
        [
          CustomValidators.lessThanOrEqualToControl(
            'minFoundationYear',
            'maxFoundationYear'
          ),
          CustomValidators.greaterThanOrEqualToControl(
            'maxFoundationYear',
            'minFoundationYear'
          )
        ]
      ),
      sortBy: new FormControl(0),
      sortDirection: new FormControl(0)
    });
  }
}
