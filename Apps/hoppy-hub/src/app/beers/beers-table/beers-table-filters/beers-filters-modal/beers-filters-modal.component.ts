import { CommonModule } from '@angular/common';
import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  inject
} from '@angular/core';
import {
  ReactiveFormsModule,
  FormGroup,
  FormControl,
  FormsModule
} from '@angular/forms';
import { Subscription } from 'rxjs';
import { AlertService } from '../../../../alert/alert.service';
import { ModalService, ModalType } from '../../../../services/modal.service';
import { BeersService } from '../../../beers.service';
import { Brewery } from '../../../../breweries/brewery.model';
import { BreweryAddress } from '../../../../breweries/brewery-address.model';

@Component({
  selector: 'app-beers-filters-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './beers-filters-modal.component.html'
})
export class BeersFiltersModalComponent implements OnInit, OnDestroy {
  private modalService = inject(ModalService);
  private beersService = inject(BeersService);
  private alertService = inject(AlertService);

  @ViewChild('beersFiltersModal') modalRef!: ElementRef;
  modalOppenedSubscription!: Subscription;
  beersFiltersForm!: FormGroup;
  breweries: Brewery[] = [
    {
      id: '123',
      name: 'Brewery 1',
      description: '',
      foundationYear: 2012,
      websiteUrl: '',
      address: new BreweryAddress('', '', '', '', '', '', '')
    },
    {
      id: '122',
      name: 'Brewery 2',
      description: '',
      foundationYear: 2012,
      websiteUrl: '',
      address: new BreweryAddress('', '', '', '', '', '', '')
    },
    {
      id: '321',
      name: 'Brewery 3',
      description: '',
      foundationYear: 2012,
      websiteUrl: '',
      address: new BreweryAddress('', '', '', '', '', '', '')
    }
  ];

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalType: ModalType) => {
        this.showModal(modalType);
      }
    );
    this.beersFiltersForm = new FormGroup({
      brewery: new FormControl('')
    });
  }

  onSubmit() {
    const selectedBrewery = this.beersFiltersForm.value.brewery;

    console.log(selectedBrewery);
  }

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
