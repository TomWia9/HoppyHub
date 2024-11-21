import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  inject
} from '@angular/core';
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
import { DataHelper } from '../../shared/data-helper';
import { BeerCardComponent } from '../../beers/beer-card/beer-card.component';

@Component({
  selector: 'app-brewery-details',
  standalone: true,
  templateUrl: './brewery-details.component.html',
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    PaginationComponent,
    BreweryBeersFiltersComponent,
    BeerCardComponent
  ]
})
export class BreweryDetailsComponent
  extends DataHelper
  implements OnInit, OnDestroy
{
  @ViewChild('details') details!: ElementRef;

  brewery!: Brewery;
  error = '';
  breweryLoading = true;
  breweryBeersLoading = true;
  routeSubscription!: Subscription;
  brewerySubscription!: Subscription;
  beersSubscription!: Subscription;
  beersParamsSubscription!: Subscription;
  beersParams = new BeersParams({
    pageSize: 9,
    pageNumber: 1,
    sortBy: 'releaseDate',
    sortDirection: 1
  });
  beers: PagedList<Beer> | undefined;
  paginationData!: Pagination;

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
              this.breweryLoading = true;
              this.brewery = brewery;
              this.error = '';
              this.scrollToDetails(-350);
              this.breweryLoading = false;
            },
            error: () => {
              this.error = 'An error occurred while loading the brewery';
              this.breweryLoading = false;
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
          this.breweryBeersLoading = true;
          this.beers = beers;
          this.paginationData = this.getPaginationData(beers);
          this.error = '';
          this.breweryBeersLoading = false;
        },
        error: () => {
          this.error = 'An error occurred while loading the beers';
          this.breweryBeersLoading = false;
        }
      });
  }

  scrollToDetails(offset: number = 0): void {
    if (this.details) {
      const elementPosition =
        this.details.nativeElement.getBoundingClientRect().top +
        window.scrollY +
        offset;

      window.scrollTo({
        top: elementPosition,
        behavior: 'smooth'
      });
    }
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
      new BeersParams({
        pageSize: 25,
        pageNumber: 1,
        sortBy: 'releaseDate',
        sortDirection: 1
      })
    );
    this.resetBreweryDetails(this.brewery.id);

    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
