import {
  Component,
  ElementRef,
  inject,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { BeerStylesService } from '../../../beer-styles/beer-styles.service';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { HttpErrorResponse } from '@angular/common/http';
import {
  FormGroup,
  FormControl,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';
import {
  finalize,
  map,
  mergeAll,
  Observable,
  of,
  Subject,
  switchMap,
  takeUntil,
  tap
} from 'rxjs';
import { BeerStyle } from '../../../beer-styles/beer-style.model';
import { BeerStylesParams } from '../../../beer-styles/beer-styles-params';
import { BeersParams } from '../../../beers/beers-params';
import { BeersService } from '../../../beers/beers.service';
import { UpsertBeerCommand } from '../../../beers/upsert-beer-command.model';
import { BreweriesParams } from '../../../breweries/breweries-params';
import { BreweriesService } from '../../../breweries/breweries.service';
import { Brewery } from '../../../breweries/brewery.model';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { CommonModule } from '@angular/common';
import { UpsertBeerImageCommand } from '../../../beers/upsert-beer-image-command.model';

@Component({
  selector: 'app-new-beer',
  standalone: true,
  imports: [
    FontAwesomeModule,
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './new-beer.component.html'
})
export class NewBeerComponent implements OnInit, OnDestroy {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  private beersService: BeersService = inject(BeersService);
  private beerStylesService: BeerStylesService = inject(BeerStylesService);
  private breweriesService: BreweriesService = inject(BreweriesService);
  private alertService: AlertService = inject(AlertService);
  private destroy$ = new Subject<void>();

  error = '';
  loading = false;
  beerStylesLoading = false;
  breweriesLoading = false;
  newBeerForm!: FormGroup;
  beerStyles: BeerStyle[] = [];
  breweries: Brewery[] = [];
  selectedImage: File | null = null;
  imageSource: string = '';

  ngOnInit(): void {
    this.initForm();
    this.fetchAllBeerStyles();
    this.fetchAllBreweries();
  }

  onFormSave(): void {
    this.loading = true;
    const upsertBeerCommand = this.newBeerForm.value as UpsertBeerCommand;

    if (!this.newBeerForm.pristine) {
      this.beersService
        .createBeer(upsertBeerCommand)
        .pipe(
          switchMap(beer => {
            if (this.selectedImage) {
              const upsertBeerImageCommand = new UpsertBeerImageCommand(
                beer.id,
                this.selectedImage
              );
              return this.beersService
                .upsertBeerImage(beer.id, upsertBeerImageCommand)
                .pipe(map(() => beer));
            }
            return of(beer);
          }),
          finalize(() => {
            this.loading = false;
          })
        )
        .subscribe({
          next: () => {
            this.alertService.openAlert(
              AlertType.Success,
              'Beer created successfully'
            );
            this.beerChanged();
            this.newBeerForm.reset();
            this.selectedImage = null;
            this.imageSource = '';
          },
          error: error => {
            this.handleError(error);
          }
        });
    }
  }

  beerChanged(): void {
    this.beersService.paramsChanged.next(
      new BeersParams({
        pageSize: 10,
        pageNumber: 1,
        sortBy: 'releaseDate',
        sortDirection: 1
      })
    );
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files) {
      const allowedTypes = ['image/jpeg', 'image/png'];

      if (!allowedTypes.includes(input.files[0].type)) {
        this.alertService.openAlert(
          AlertType.Error,
          'Only JPG and PNG files are allowed.'
        );
        input.value = '';
        this.selectedImage = null;
        return;
      }
      this.selectedImage = input.files[0];
      const imageUrl = URL.createObjectURL(this.selectedImage);
      this.imageSource = imageUrl;
    }
  }

  private fetchAllBeerStyles() {
    this.beerStylesLoading = true;

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
            this.beerStylesLoading = false;
          },
          error: () => {
            this.error = 'An error occurred while loading the beer styles';
            this.beerStylesLoading = false;
          }
        })
      )
      .subscribe();
  }

  private fetchAllBreweries() {
    this.breweriesLoading = true;

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
            this.breweriesLoading = false;
          },
          error: () => {
            this.error = 'An error occurred while loading the breweries';
            this.breweriesLoading = false;
          }
        })
      )
      .subscribe();
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = null;

    if (error.error) {
      const firstKey = Object.keys(error.error?.errors)[0] ?? null;
      const firstValueArray = error.error?.errors[firstKey] as string[];
      errorMessage = firstValueArray[0];
    }

    if (!errorMessage) {
      this.alertService.openAlert(AlertType.Error, 'Something went wrong');
    } else {
      this.alertService.openAlert(AlertType.Error, errorMessage);
    }

    this.loading = false;
  }

  private initForm(): void {
    this.newBeerForm = new FormGroup({
      name: new FormControl('', [Validators.required, Validators.minLength(2)]),
      description: new FormControl('', Validators.maxLength(500)),
      beerStyleId: new FormControl(null, Validators.required),
      breweryId: new FormControl(null, Validators.required),
      releaseDate: new FormControl(null, Validators.required),
      alcoholByVolume: new FormControl(null, [
        Validators.min(0),
        Validators.max(100)
      ]),
      blg: new FormControl(null, [Validators.min(0), Validators.max(100)]),
      ibu: new FormControl(null, [Validators.min(0), Validators.max(120)]),
      composition: new FormControl('', Validators.maxLength(1000))
    });
  }

  ngOnDestroy(): void {
    if (this.imageSource) {
      URL.revokeObjectURL(this.imageSource);
    }
    this.destroy$.next();
    this.destroy$.complete();
  }
}
