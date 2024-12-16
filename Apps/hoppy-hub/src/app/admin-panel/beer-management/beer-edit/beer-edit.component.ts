import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subscription, map } from 'rxjs';
import { Beer } from '../../../beers/beer.model';
import { BeersService } from '../../../beers/beers.service';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { BeerStyle } from '../../../beer-styles/beer-style.model';
import { BeerStylesService } from '../../../beer-styles/beer-styles.service';
import { BeerStylesParams } from '../../../beer-styles/beer-styles-params';
import { PagedList } from '../../../shared/paged-list';
import { BreweriesService } from '../../../breweries/breweries.service';
import { BreweriesParams } from '../../../breweries/breweries-params';
import { Brewery } from '../../../breweries/brewery.model';
import { UpsertBeerCommand } from '../../../beers/upsert-beer-command.model';
import { HttpErrorResponse } from '@angular/common/http';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';

@Component({
  selector: 'app-beer-edit',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    RouterModule,
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './beer-edit.component.html'
})
export class BeerEditComponent implements OnInit, OnDestroy {
  private route: ActivatedRoute = inject(ActivatedRoute);
  private beersService: BeersService = inject(BeersService);
  private beerStylesService = inject(BeerStylesService);
  private breweriesService = inject(BreweriesService);
  private alertService = inject(AlertService);
  private routeSubscription!: Subscription;
  private beerSubscription!: Subscription;
  private beerStylesSubscription!: Subscription;
  private breweriesSubscription!: Subscription;
  private updateBeerSubscription!: Subscription;

  beer!: Beer;
  error = '';
  loading = true;
  beerStylesLoading = false;
  breweriesLoading = false;
  beerForm!: FormGroup;
  beerStyles: BeerStyle[] = [];
  breweries: Brewery[] = [];

  ngOnInit(): void {
    this.fetchAllBeerStyles();
    this.fetchAllBreweries();
    this.getBeer();
  }

  getBeer(): void {
    this.loading = true;
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(beerId => {
        this.beerSubscription = this.beersService
          .getBeerById(beerId as string)
          .subscribe({
            next: (beer: Beer) => {
              this.beer = beer;
              this.initForm(beer);
              this.error = '';
              window.scrollTo({ top: 0, behavior: 'smooth' });
              this.loading = false;
            },
            error: () => {
              this.error = 'An error occurred while loading the beer';
              this.loading = false;
            }
          });
      });
  }

  private fetchAllBeerStyles(
    pageNumber: number = 1,
    allBeerStyles: BeerStyle[] = []
  ): void {
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

  onBeerSave(): void {
    console.log(this.beerForm);

    if (this.beerForm.valid) {
      this.loading = true;
      const upsertBeerCommand = this.beerForm.value as UpsertBeerCommand;
      console.log(upsertBeerCommand);

      upsertBeerCommand.id = this.beer.id;
      this.updateBeerSubscription = this.beersService
        .UpdateBeer(this.beer.id, upsertBeerCommand)
        .subscribe({
          next: () => {
            this.getBeer();
            this.alertService.openAlert(AlertType.Success, 'Beer updated');
            this.loading = false;
          },
          error: error => {
            this.handleError(error);
          }
        });
    }
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

  private initForm(beer: Beer): void {
    this.beerForm = new FormGroup({
      name: new FormControl(beer.name, [
        Validators.required,
        Validators.minLength(2)
      ]),
      description: new FormControl(beer.description, Validators.maxLength(500)),
      beerStyleId: new FormControl(beer.beerStyle.id, Validators.required),
      breweryId: new FormControl(beer.brewery.id, Validators.required),
      releaseDate: new FormControl(beer.releaseDate, Validators.required),
      alcoholByVolume: new FormControl(beer.alcoholByVolume, [
        Validators.min(0),
        Validators.max(100)
      ]),
      blg: new FormControl(beer.blg, [Validators.min(0), Validators.max(100)]),
      ibu: new FormControl(beer.ibu, [Validators.min(0), Validators.max(120)]),
      composition: new FormControl(beer.composition, Validators.maxLength(1000))
    });
  }

  ngOnDestroy(): void {
    if (this.beerSubscription) {
      this.beerSubscription.unsubscribe();
    }
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
    if (this.beerStylesSubscription) {
      this.beerStylesSubscription.unsubscribe();
    }
    if (this.breweriesSubscription) {
      this.breweriesSubscription.unsubscribe();
    }
    if (this.updateBeerSubscription) {
      this.updateBeerSubscription.unsubscribe();
    }
  }
}
