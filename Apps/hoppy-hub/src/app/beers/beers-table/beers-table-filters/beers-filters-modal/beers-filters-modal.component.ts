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
import { map, mergeAll, Observable, of, Subject, takeUntil, tap } from 'rxjs';
import { ModalService } from '../../../../services/modal.service';
import { BeersService } from '../../../beers.service';
import { Brewery } from '../../../../breweries/brewery.model';
import { BreweriesService } from '../../../../breweries/breweries.service';
import { BreweriesParams } from '../../../../breweries/breweries-params';
import { LoadingSpinnerComponent } from '../../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../../shared-components/error-message/error-message.component';
import { BeerStylesService } from '../../../../beer-styles/beer-styles.service';
import { BeerStylesParams } from '../../../../beer-styles/beer-styles-params';
import { BeerStyle } from '../../../../beer-styles/beer-style.model';
import { BeersParams } from '../../../beers-params';
import { CustomValidators } from '../../../../shared/custom-validators';
import { ModalModel } from '../../../../shared/modal-model';
import { ModalType } from '../../../../shared/model-type';

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
  private destroy$ = new Subject<void>();

  beersFiltersForm!: FormGroup;
  breweries: Brewery[] = [];
  beerStyles: BeerStyle[] = [];
  error = '';
  loading = true;

  sortOptions = BeersParams.sortOptions;

  ngOnInit(): void {
    this.modalService.modalOpened
      .pipe(
        takeUntil(this.destroy$),
        tap((modalModel: ModalModel) => {
          this.onShowModal(modalModel);
        })
      )
      .subscribe();

    this.fetchAllBreweries();
    this.fetchAllBeerStyles();

    this.beersFiltersForm = this.getBeerFiltersForm();
  }

  onSubmit(): void {
    const beersParams = new BeersParams({
      pageSize: 25,
      pageNumber: 1,
      sortBy: this.sortOptions[this.beersFiltersForm.value.sortBy].value,
      sortDirection:
        this.sortOptions[this.beersFiltersForm.value.sortBy].direction,
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

  onShowModal(modalModel: ModalModel): void {
    if (modalModel.modalType === ModalType.BeersFilters && this.modalRef) {
      (this.modalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  onClearFilters(): void {
    this.beersFiltersForm.reset();
  }

  private fetchAllBreweries() {
    this.loading = true;

    const fetchPage = (
      pageNumber: number,
      accumulator: Brewery[] = []
    ): Observable<Brewery[]> => {
      return this.breweriesService
        .getBreweries(new BreweriesParams({ pageSize: 50, pageNumber }))
        .pipe(
          map(response => {
            const newAccumulator = [...accumulator, ...response.items];
            if (response.HasNext) {
              return fetchPage(pageNumber + 1, newAccumulator);
            }
            return of(newAccumulator);
          }),
          mergeAll()
        );
    };

    fetchPage(1)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: breweries => {
            this.breweries = breweries;
            this.error = '';
          },
          error: () => {
            this.error = 'An error occurred while loading the breweries';
          },
          complete: () => {
            this.loading = false;
          }
        })
      )
      .subscribe();
  }

  private fetchAllBeerStyles() {
    this.loading = true;

    const fetchPage = (
      pageNumber: number,
      accumulator: BeerStyle[] = []
    ): Observable<BeerStyle[]> => {
      return this.beerStylesService
        .getBeerStyles(new BeerStylesParams({ pageSize: 50, pageNumber }))
        .pipe(
          map(response => {
            const newAccumulator = [...accumulator, ...response.items];
            if (response.HasNext) {
              return fetchPage(pageNumber + 1, newAccumulator);
            }
            return of(newAccumulator);
          }),
          mergeAll()
        );
    };

    fetchPage(1)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: styles => {
            this.beerStyles = styles;
            this.error = '';
          },
          error: () => {
            this.error = 'An error occurred while loading the beer styles';
          },
          complete: () => {
            this.loading = false;
          }
        })
      )
      .subscribe();
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

      sortBy: new FormControl(0),
      sortDirection: new FormControl(0)
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
