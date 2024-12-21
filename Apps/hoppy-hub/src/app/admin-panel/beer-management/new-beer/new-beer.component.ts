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
import { Subscription } from 'rxjs';
import { BeerStyle } from '../../../beer-styles/beer-style.model';
import { BeerStylesParams } from '../../../beer-styles/beer-styles-params';
import { BeersParams } from '../../../beers/beers-params';
import { BeersService } from '../../../beers/beers.service';
import { UpsertBeerCommand } from '../../../beers/upsert-beer-command.model';
import { BreweriesParams } from '../../../breweries/breweries-params';
import { BreweriesService } from '../../../breweries/breweries.service';
import { Brewery } from '../../../breweries/brewery.model';
import { PagedList } from '../../../shared/paged-list';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { CommonModule } from '@angular/common';
import { Beer } from '../../../beers/beer.model';

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
  private routeSubscription!: Subscription;
  private beerStylesSubscription!: Subscription;
  private breweriesSubscription!: Subscription;
  private newBeerSubscription!: Subscription;

  error = '';
  loading = false;
  beerStylesLoading = false;
  breweriesLoading = false;
  newBeerForm!: FormGroup;
  beerStyles: BeerStyle[] = [];
  breweries: Brewery[] = [];
  createdBeer: Beer | null = null;

  ngOnInit(): void {
    this.initForm();
    this.fetchAllBeerStyles();
    this.fetchAllBreweries();
  }

  onFormSave(): void {
    this.loading = true;

    if (!this.newBeerForm.pristine) {
      const upsertBeerCommand = this.newBeerForm.value as UpsertBeerCommand;

      this.newBeerSubscription = this.beersService
        .CreateBeer(upsertBeerCommand)
        .subscribe({
          next: (beer: Beer) => {
            this.createdBeer = beer;
            this.loading = false;
            this.alertService.openAlert(
              AlertType.Success,
              'Beer created successfully'
            );
            this.beerChanged();
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

  private fetchAllBeerStyles(
    pageNumber: number = 1,
    allBeerStyles: BeerStyle[] = []
  ): void {
    this.beerStylesLoading = true;
    this.beerStylesSubscription = this.beerStylesService
      .getBeerStyles(
        new BeerStylesParams({ pageSize: 50, pageNumber: pageNumber })
      )
      .subscribe({
        next: (beerStyles: PagedList<BeerStyle>) => {
          allBeerStyles.push(...beerStyles.items);
          if (beerStyles.HasNext) {
            this.fetchAllBeerStyles(pageNumber + 1, allBeerStyles);
          } else {
            this.beerStyles = allBeerStyles;
            this.error = '';
            this.beerStylesLoading = false;
          }
        },
        error: () => {
          this.error = 'An error occurred while loading the beer styles';
          this.beerStylesLoading = false;
        }
      });
  }

  private fetchAllBreweries(
    pageNumber: number = 1,
    allBreweries: Brewery[] = []
  ): void {
    this.breweriesLoading = true;
    this.breweriesSubscription = this.breweriesService
      .getBreweries(
        new BreweriesParams({ pageSize: 50, pageNumber: pageNumber })
      )
      .subscribe({
        next: (breweries: PagedList<Brewery>) => {
          allBreweries.push(...breweries.items);
          if (breweries.HasNext) {
            this.fetchAllBreweries(pageNumber + 1, allBreweries);
          } else {
            this.breweries = allBreweries;
            this.error = '';
            this.breweriesLoading = false;
          }
        },
        error: () => {
          this.error = 'An error occurred while loading the breweries';
          this.breweriesLoading = false;
        }
      });
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
      beerStyleId: new FormControl('', Validators.required),
      breweryId: new FormControl('', Validators.required),
      releaseDate: new FormControl(new Date(), Validators.required),
      alcoholByVolume: new FormControl('', [
        Validators.min(0),
        Validators.max(100)
      ]),
      blg: new FormControl('', [Validators.min(0), Validators.max(100)]),
      ibu: new FormControl('', [Validators.min(0), Validators.max(120)]),
      composition: new FormControl('', Validators.maxLength(1000))
    });
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
    if (this.beerStylesSubscription) {
      this.beerStylesSubscription.unsubscribe();
    }
    if (this.breweriesSubscription) {
      this.breweriesSubscription.unsubscribe();
    }
    if (this.newBeerSubscription) {
      this.newBeerSubscription.unsubscribe();
    }
  }
}
