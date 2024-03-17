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
import { BreweriesService } from '../../../../breweries/breweries.service';
import { PagedList } from '../../../../shared/paged-list';
import { BreweriesParams } from '../../../../breweries/breweries-params';
import { LoadingSpinnerComponent } from '../../../../loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../../shared-components/error-message/error-message.component';
import { BeerStylesService } from '../../../../beer-styles/beer-styles.service';
import { BeerStylesParams } from '../../../../beer-styles/beer-styles-params';
import { BeerStyle } from '../../../../beer-styles/beer-style.model';

@Component({
  selector: 'app-beers-filters-modal',
  standalone: true,
  templateUrl: './beers-filters-modal.component.html',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    LoadingSpinnerComponent,
    ErrorMessageComponent
  ]
})
export class BeersFiltersModalComponent implements OnInit, OnDestroy {
  private modalService = inject(ModalService);
  private beersService = inject(BeersService);
  private breweriesService = inject(BreweriesService);
  private beerStylesService = inject(BeerStylesService);
  private alertService = inject(AlertService);

  @ViewChild('beersFiltersModal') modalRef!: ElementRef;
  modalOppenedSubscription!: Subscription;
  beersFiltersForm!: FormGroup;
  breweries: Brewery[] = [];
  beerStyles: BeerStyle[] = [];
  error = '';
  loading = true;
  getBreweriesSubscription!: Subscription;
  getBeerStylesSubscription!: Subscription;

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalType: ModalType) => {
        this.showModal(modalType);
      }
    );

    this.fetchAllBreweries();
    this.fetchAllBeerStyles();

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
    //TODO: this.beersFiltersForm.value.brewery = 'Brewery';
    if (this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  showModal(modalType: ModalType) {
    if (modalType === ModalType.BeersFilters && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  fetchAllBreweries(pageNumber: number = 1, allBreweries: Brewery[] = []) {
    this.getBreweriesSubscription = this.breweriesService
      .getBreweries(new BreweriesParams(50, pageNumber))
      .subscribe({
        next: (breweries: PagedList<Brewery>) => {
          this.loading = true;
          allBreweries.push(...breweries.items);
          if (breweries.HasNext) {
            this.fetchAllBreweries(pageNumber + 1, allBreweries);
          } else {
            this.breweries = allBreweries;
            this.loading = false;
          }
        },
        error: () => {
          this.error = 'An error occurred while loading the breweries';
          this.loading = false;
        }
      });
  }

  fetchAllBeerStyles(pageNumber: number = 1, allBeerStyles: BeerStyle[] = []) {
    this.getBeerStylesSubscription = this.beerStylesService
      .getBeerStyles(new BeerStylesParams(50, pageNumber))
      .subscribe({
        next: (beerStyles: PagedList<BeerStyle>) => {
          this.loading = true;
          allBeerStyles.push(...beerStyles.items);
          if (beerStyles.HasNext) {
            this.fetchAllBeerStyles(pageNumber + 1, allBeerStyles);
          } else {
            this.beerStyles = allBeerStyles;
            this.loading = false;
          }
        },
        error: () => {
          this.error = 'An error occurred while loading the breweries';
          this.loading = false;
        }
      });
  }

  ngOnDestroy(): void {
    this.modalOppenedSubscription.unsubscribe();
    this.getBreweriesSubscription.unsubscribe();
    this.getBeerStylesSubscription.unsubscribe();
  }
}
