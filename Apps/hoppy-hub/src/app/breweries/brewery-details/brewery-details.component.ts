import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { ActivatedRoute } from '@angular/router';
import { Subscription, map } from 'rxjs';
import { BreweriesService } from '../breweries.service';
import { Brewery } from '../brewery.model';
import { BeersService } from '../../beers/beers.service';
import { Beer } from '../../beers/beer.model';
import { PagedList } from '../../shared/paged-list';
import { BeersParams } from '../../beers/beers-params';
import { Pagination } from '../../shared/pagination';
import { PaginationComponent } from '../../shared-components/pagination/pagination.component';
import { BreweryBeersFiltersComponent } from './brewery-beers-filters/brewery-beers-filters.component';

@Component({
  selector: 'app-brewery-details',
  standalone: true,
  templateUrl: './brewery-details.component.html',
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    PaginationComponent,
    BreweryBeersFiltersComponent
  ]
})
export class BreweryDetailsComponent implements OnInit, OnDestroy {
  brewery!: Brewery;
  error = '';
  loading = true;
  routeSubscription!: Subscription;
  brewerySubscription!: Subscription;
  beersSubscription!: Subscription;
  beersParamsSubscription!: Subscription;
  beersParams = new BeersParams(9, 1, 'ReleaseDate', 1);
  beers: PagedList<Beer> | undefined;

  private route: ActivatedRoute = inject(ActivatedRoute);
  private breweriesService: BreweriesService = inject(BreweriesService);
  private beersService: BeersService = inject(BeersService);

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(breweryId => {
        this.resetBreweryDetails(breweryId as string);

        this.brewerySubscription = this.breweriesService
          .getBreweryById(breweryId as string)
          .subscribe({
            next: (brewery: Brewery) => {
              this.loading = true;
              this.brewery = brewery;
              this.error = '';
              this.loading = false;
            },
            error: () => {
              this.error = 'An error occurred while loading the brewery';
              this.loading = false;
            }
          });

        this.beersParamsSubscription =
          this.beersService.paramsChanged.subscribe((params: BeersParams) => {
            this.beersParams = params;
            this.getBeers();
          });
      });
  }

  private getBeers(): void {
    this.beersSubscription = this.beersService
      .getBeers(this.beersParams)
      .subscribe({
        next: (beers: PagedList<Beer>) => {
          this.loading = true;
          this.beers = beers;
          this.error = '';
          this.loading = false;
        },
        error: () => {
          this.error = 'An error occurred while loading the beers';
          this.loading = false;
        }
      });
  }

  getPaginationData(): Pagination {
    if (this.beers) {
      return {
        CurrentPage: this.beers.CurrentPage,
        HasNext: this.beers.HasNext,
        HasPrevious: this.beers.HasPrevious,
        TotalPages: this.beers.TotalPages,
        TotalCount: this.beers.TotalCount
      };
    }

    return {
      CurrentPage: 0,
      HasNext: false,
      HasPrevious: false,
      TotalPages: 0,
      TotalCount: 0
    };
  }

  private resetBreweryDetails(breweryId: string): void {
    if (this.brewerySubscription) {
      this.brewerySubscription.unsubscribe();
    }
    if (this.beersParamsSubscription) {
      this.beersParamsSubscription.unsubscribe();
    }
    if (this.beersSubscription) {
      this.beersSubscription.unsubscribe();
    }

    this.beersParams.breweryId = breweryId;
    this.beersParams.pageNumber = 1;
    this.beersParams.searchQuery = '';
    this.beersParams.sortBy = 'ReleaseDate';
    this.beersParams.sortDirection = 1;
    this.beersService.paramsChanged.next(this.beersParams);
  }

  ngOnDestroy(): void {
    this.beersService.paramsChanged.next(
      new BeersParams(25, 1, 'ReleaseDate', 1)
    );
    this.resetBreweryDetails(this.brewery.id);

    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
