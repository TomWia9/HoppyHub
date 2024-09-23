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
  FormsModule,
  Validators
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
import { CustomValidators } from '../../../../shared/custom-validators';

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
  @ViewChild('beersFiltersModal') modalRef!: ElementRef;

  private modalService = inject(ModalService);
  private beersService = inject(BeersService);
  private breweriesService = inject(BreweriesService);
  private beerStylesService = inject(BeerStylesService);

  beersFiltersForm!: FormGroup;
  breweries: Brewery[] = [];
  beerStyles: BeerStyle[] = [];
  error = '';
  loading = true;
  modalOppenedSubscription!: Subscription;
  getBreweriesSubscription!: Subscription;
  getBeerStylesSubscription!: Subscription;
  sortOptions = BeersParams.sortOptions;
  selectedSortOptionIndex: number = 0;

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalType: ModalType) => {
        this.onShowModal(modalType);
      }
    );

    this.fetchAllBreweries();
    this.fetchAllBeerStyles();

    this.beersFiltersForm = this.getBeerFiltersForm();
  }

  onSubmit(): void {
    const beersParams = new BeersParams({
      pageSize: 25,
      pageNumber: 1,
      sortBy: this.sortOptions[this.selectedSortOptionIndex].value,
      sortDirection: this.sortOptions[this.selectedSortOptionIndex].direction,
      breweryId: this.beersFiltersForm.value.brewery,
      beerStyleId: this.beersFiltersForm.value.beerStyle,
      minAlcoholByVolume: this.beersFiltersForm.value.abv.minAbv,
      maxAlcoholByVolume: this.beersFiltersForm.value.abv.maxAbv,
      minExtract: this.beersFiltersForm.value.extract.minExtract,
      maxExtract: this.beersFiltersForm.value.extract.maxExtract,
      minIbu: this.beersFiltersForm.value.ibu.minIbu,
      maxIbu: this.beersFiltersForm.value.ibu.maxIbu,
      minReleaseDate: this.beersFiltersForm.value.releaseDates.minReleaseDate,
      maxReleaseDate: this.beersFiltersForm.value.releaseDates.maxReleaseDate,
      minRating: this.beersFiltersForm.value.rating.minRating,
      maxRating: this.beersFiltersForm.value.rating.maxRating,
      minFavoritesCount:
        this.beersFiltersForm.value.favorites.minFavoritesCount,
      maxFavoritesCount:
        this.beersFiltersForm.value.favorites.maxFavoritesCount,
      minOpinionsCount: this.beersFiltersForm.value.opinions.minOpinionsCount,
      maxOpinionsCount: this.beersFiltersForm.value.opinions.maxOpinionsCount
    });

    this.beersService.paramsChanged.next(beersParams);

    this.onModalHide();
  }

  onModalHide(): void {
    if (this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onShowModal(modalType: ModalType): void {
    if (modalType === ModalType.BeersFilters && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  onClearFilters(): void {
    this.beersFiltersForm.reset();
  }

  private fetchAllBreweries(
    pageNumber: number = 1,
    allBreweries: Brewery[] = []
  ): void {
    this.getBreweriesSubscription = this.breweriesService
      .getBreweries(
        new BreweriesParams({ pageSize: 50, pageNumber: pageNumber })
      )
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

  private fetchAllBeerStyles(
    pageNumber: number = 1,
    allBeerStyles: BeerStyle[] = []
  ): void {
    this.getBeerStylesSubscription = this.beerStylesService
      .getBeerStyles(
        new BeerStylesParams({ pageSize: 50, pageNumber: pageNumber })
      )
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

  private getBeerFiltersForm(): FormGroup {
    return new FormGroup({
      brewery: new FormControl(''),
      abv: new FormGroup(
        {
          minAbv: new FormControl('', [Validators.min(0), Validators.max(100)]),
          maxAbv: new FormControl('', [Validators.min(0), Validators.max(100)])
        },
        [
          CustomValidators.lessThanOrEqualToControl('minAbv', 'maxAbv'),
          CustomValidators.greaterThanOrEqualToControl('maxAbv', 'minAbv')
        ]
      ),
      extract: new FormGroup(
        {
          minExtract: new FormControl('', [
            Validators.min(0),
            Validators.max(100)
          ]),
          maxExtract: new FormControl('', [
            Validators.min(0),
            Validators.max(100)
          ])
        },
        [
          CustomValidators.lessThanOrEqualToControl('minExtract', 'maxExtract'),
          CustomValidators.greaterThanOrEqualToControl(
            'maxExtract',
            'minExtract'
          )
        ]
      ),
      ibu: new FormGroup(
        {
          minIbu: new FormControl('', [Validators.min(0), Validators.max(200)]),
          maxIbu: new FormControl('', [Validators.min(0), Validators.max(200)])
        },
        [
          CustomValidators.lessThanOrEqualToControl('minIbu', 'maxIbu'),
          CustomValidators.greaterThanOrEqualToControl('maxIbu', 'minIbu')
        ]
      ),
      beerStyle: new FormControl(''),
      rating: new FormGroup(
        {
          minRating: new FormControl('', [
            Validators.min(0),
            Validators.max(10)
          ]),
          maxRating: new FormControl('', [
            Validators.min(0),
            Validators.max(10)
          ])
        },
        [
          CustomValidators.lessThanOrEqualToControl('minRating', 'maxRating'),
          CustomValidators.greaterThanOrEqualToControl('maxRating', 'minRating')
        ]
      ),
      favorites: new FormGroup(
        {
          minFavoritesCount: new FormControl('', [Validators.min(0)]),
          maxFavoritesCount: new FormControl('', [Validators.min(0)])
        },
        [
          CustomValidators.lessThanOrEqualToControl(
            'minFavoritesCount',
            'maxFavoritesCount'
          ),
          CustomValidators.greaterThanOrEqualToControl(
            'maxFavoritesCount',
            'minFavoritesCount'
          )
        ]
      ),
      opinions: new FormGroup(
        {
          minOpinionsCount: new FormControl('', [Validators.min(0)]),
          maxOpinionsCount: new FormControl('', [Validators.min(0)])
        },
        [
          CustomValidators.lessThanOrEqualToControl(
            'minOpinionsCount',
            'maxOpinionsCount'
          ),
          CustomValidators.greaterThanOrEqualToControl(
            'maxOpinionsCount',
            'minOpinionsCount'
          )
        ]
      ),
      releaseDates: new FormGroup(
        {
          minReleaseDate: new FormControl(''),
          maxReleaseDate: new FormControl('')
        },
        [
          CustomValidators.lessThanOrEqualToControl(
            'minReleaseDate',
            'maxReleaseDate'
          ),
          CustomValidators.greaterThanOrEqualToControl(
            'maxReleaseDate',
            'minReleaseDate'
          )
        ]
      ),

      sortBy: new FormControl(''),
      sortDirection: new FormControl('')
    });
  }
}
