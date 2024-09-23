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
import { ModalService, ModalType } from '../../../../services/modal.service';
import { CustomValidators } from '../../../../shared/custom-validators';
import { BreweriesParams } from '../../../breweries-params';
import { BreweriesService } from '../../../breweries.service';
import { Brewery } from '../../../brewery.model';
import { LoadingSpinnerComponent } from '../../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../../shared-components/error-message/error-message.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-breweries-filters-modal',
  standalone: true,
  templateUrl: './breweries-filters-modal.component.html',
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    ReactiveFormsModule,
    CommonModule
  ]
})
export class BreweriesFiltersModalComponent implements OnInit, OnDestroy {
  @ViewChild('breweriesFiltersModal') modalRef!: ElementRef;

  private modalService = inject(ModalService);
  private breweriesService = inject(BreweriesService);

  breweriesFiltersForm!: FormGroup;
  breweries: Brewery[] = [];
  error = '';
  getBreweriesSubscription!: Subscription;
  modalOppenedSubscription!: Subscription;
  sortOptions = BreweriesParams.sortOptions;
  selectedSortOptionIndex: number = 0;
  currentYear = new Date().getFullYear();

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalType: ModalType) => {
        this.onShowModal(modalType);
      }
    );

    this.breweriesFiltersForm = this.getBreweriesFiltersForm();
  }

  onSubmit(): void {
    const breweriesParams = new BreweriesParams({
      pageSize: 25,
      pageNumber: 1,
      sortBy: this.sortOptions[this.selectedSortOptionIndex].value,
      sortDirection: this.sortOptions[this.selectedSortOptionIndex].direction,
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

  onShowModal(modalType: ModalType): void {
    if (modalType === ModalType.BreweriesFilters && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  onClearFilters(): void {
    this.breweriesFiltersForm.reset();
  }

  ngOnDestroy(): void {
    this.modalOppenedSubscription.unsubscribe();
  }

  getBreweriesFiltersForm(): FormGroup {
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
      sortBy: new FormControl(''),
      sortDirection: new FormControl('')
    });
  }
}
