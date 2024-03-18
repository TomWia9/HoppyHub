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
import { ModalService, ModalType } from '../../../../services/modal.service';
import { BeersService } from '../../../beers.service';
import { Brewery } from '../../../../breweries/brewery.model';
import { BreweriesService } from '../../../../breweries/breweries.service';
import { PagedList } from '../../../../shared/paged-list';
import { BreweriesParams } from '../../../../breweries/breweries-params';
import { LoadingSpinnerComponent } from '../../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../../shared-components/error-message/error-message.component';
import { BeerStylesService } from '../../../../beer-styles/beer-styles.service';
import { BeerStylesParams } from '../../../../beer-styles/beer-styles-params';
import { BeerStyle } from '../../../../beer-styles/beer-style.model';
import { BeersParams } from '../../../beers-params';

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

  @ViewChild('beersFiltersModal') modalRef!: ElementRef;
  modalOppenedSubscription!: Subscription;
  beersFiltersForm!: FormGroup;
  breweries: Brewery[] = [];
  beerStyles: BeerStyle[] = [];
  error = '';
  loading = true;
  getBreweriesSubscription!: Subscription;
  getBeerStylesSubscription!: Subscription;
  sortByOptions: string[] = [
    'Name',
    'Brewery',
    'Beer style',
    'Alcohol by volume',
    'Blg',
    'Ibu',
    'Rating',
    'Opinions count',
    'Favorites count',
    'Release date'
  ];
  sortDirectionOptions: string[] = ['Asc', 'Desc'];

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalType: ModalType) => {
        this.onShowModal(modalType);
      }
    );

    this.fetchAllBreweries();
    this.fetchAllBeerStyles();

    this.beersFiltersForm = new FormGroup({
      brewery: new FormControl(''),
      minAbv: new FormControl(),
      maxAbv: new FormControl(),
      minExtract: new FormControl(),
      maxExtract: new FormControl(),
      minIbu: new FormControl(),
      maxIbu: new FormControl(),
      beerStyle: new FormControl(''),
      minRating: new FormControl(),
      maxRating: new FormControl(),
      minFavoritesCount: new FormControl(),
      maxFavoritesCount: new FormControl(),
      minOpinionsCount: new FormControl(),
      maxOpinionsCount: new FormControl(),
      minReleaseDate: new FormControl(),
      maxReleaseDate: new FormControl(),
      sortBy: new FormControl(''),
      sortDirection: new FormControl('')
    });
  }

  onSubmit() {
    const beersParams = new BeersParams(
      25,
      1,
      this.beersFiltersForm.value.sortBy?.replace(/\s/g, ''),
      this.beersFiltersForm.value.sortDirection,
      undefined,
      undefined,
      this.beersFiltersForm.value.brewery,
      this.beersFiltersForm.value.beerStyle,
      this.beersFiltersForm.value.minAbv,
      this.beersFiltersForm.value.maxAbv,
      this.beersFiltersForm.value.minExtract,
      this.beersFiltersForm.value.maxExtract,
      this.beersFiltersForm.value.minIbu,
      this.beersFiltersForm.value.maxIbu,
      this.beersFiltersForm.value.minReleaseDate,
      this.beersFiltersForm.value.maxReleaseDate,
      this.beersFiltersForm.value.minRating,
      this.beersFiltersForm.value.maxRating,
      this.beersFiltersForm.value.minFavoritesCount,
      this.beersFiltersForm.value.maxFavoritesCount,
      this.beersFiltersForm.value.minOpinionsCount,
      this.beersFiltersForm.value.maxOpinionsCount
    );

    this.beersService.paramsChanged.next(beersParams);

    this.onModalHide();
  }

  onModalHide() {
    if (this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onShowModal(modalType: ModalType) {
    if (modalType === ModalType.BeersFilters && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  onClearFilters() {
    this.beersFiltersForm.reset();
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
            this.error = '';
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
            this.error = '';
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
