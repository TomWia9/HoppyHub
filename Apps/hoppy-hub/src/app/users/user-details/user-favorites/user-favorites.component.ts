import { Component, inject, Input, OnChanges, OnDestroy } from '@angular/core';
import { FavoritesService } from '../../../favorites/favorites.service';
import { FavoritesParams } from '../../../favorites/favorites-params';
import { Beer } from '../../../beers/beer.model';
import { Subscription, switchMap, forkJoin, of } from 'rxjs';
import { BeersService } from '../../../beers/beers.service';
import { DataHelper } from '../../../shared/data-helper';
import { PagedList } from '../../../shared/paged-list';
import { Pagination } from '../../../shared/pagination';
import { User } from '../../user.model';
import { BeerCardComponent } from '../../../beers/beer-card/beer-card.component';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { UserFavoritesFiltersComponent } from './user-favorites-filters/user-favorites-filters.component';

@Component({
  selector: 'app-user-favorites',
  standalone: true,
  imports: [
    BeerCardComponent,
    PaginationComponent,
    LoadingSpinnerComponent,
    UserFavoritesFiltersComponent
  ],
  templateUrl: './user-favorites.component.html'
})
export class UserFavoritesComponent
  extends DataHelper
  implements OnChanges, OnDestroy
{
  @Input({ required: true }) user!: User;

  private favoritesService: FavoritesService = inject(FavoritesService);
  private beersService: BeersService = inject(BeersService);
  private favoriteBeersParamsSubscription!: Subscription;
  private getFavoriteBeersSubscription!: Subscription;

  sortOptions = FavoritesParams.sortOptions;
  selectedSortOptionIndex: number = 0;
  favoriteBeersParams = new FavoritesParams({
    pageSize: 6,
    pageNumber: 1,
    sortBy: 'LastModified',
    sortDirection: 1
  });
  favoriteBeers: Beer[] = [];
  paginationData!: Pagination;
  error = '';
  loading = true;

  ngOnChanges(): void {
    this.refreshFavoriteBeers();
  }

  refreshFavoriteBeers(): void {
    this.favoriteBeersParams.userId = this.user.id;
    this.favoritesService.paramsChanged.next(this.favoriteBeersParams);
    this.favoriteBeersParamsSubscription =
      this.favoritesService.paramsChanged.subscribe(
        (params: FavoritesParams) => {
          this.favoriteBeersParams = params;
          this.favoriteBeersParams.userId = this.user.id;
          this.getUserFavoriteBeers();
        }
      );
  }

  onSort(): void {
    this.favoriteBeersParams.pageNumber = 1;
    this.favoriteBeersParams.sortBy =
      this.sortOptions[this.selectedSortOptionIndex].value;
    this.favoriteBeersParams.sortDirection =
      this.sortOptions[this.selectedSortOptionIndex].direction;
    this.favoritesService.paramsChanged.next(this.favoriteBeersParams);
  }

  onFiltersClear(): void {
    this.selectedSortOptionIndex = 0;
    this.favoriteBeersParams = new FavoritesParams({
      pageSize: 6,
      pageNumber: 1,
      sortBy: 'created',
      sortDirection: 1
    });

    if (
      JSON.stringify(this.favoritesService.paramsChanged.value) !=
      JSON.stringify(this.favoriteBeersParams)
    ) {
      this.favoritesService.paramsChanged.next(this.favoriteBeersParams);
    }
  }

  private getUserFavoriteBeers(): void {
    this.getFavoriteBeersSubscription = this.favoritesService
      .getFavorites(this.favoriteBeersParams)
      .pipe(
        switchMap((favoriteBeers: PagedList<Beer>) => {
          const beerRequests = favoriteBeers.items.map(favoriteBeer =>
            this.beersService.getBeerById(favoriteBeer.id)
          );

          this.paginationData = this.getPaginationData(favoriteBeers);

          if (favoriteBeers.items.length === 0) {
            return of([]);
          }

          return forkJoin(beerRequests);
        })
      )
      .subscribe({
        next: (favoriteBeers: Beer[]) => {
          this.loading = true;
          this.favoriteBeers = favoriteBeers;
          this.error = '';
          this.loading = false;
        },
        error: error => {
          this.error =
            'An error occurred while loading the user favorite beers';

          if (error.error && error.error.errors) {
            const errorMessage = this.getValidationErrorMessage(
              error.error.errors
            );
            this.error += errorMessage;
          }

          this.loading = false;
        }
      });
  }

  ngOnDestroy(): void {
    if (this.favoriteBeersParamsSubscription) {
      this.favoriteBeersParamsSubscription.unsubscribe();
    }
    if (this.getFavoriteBeersSubscription) {
      this.getFavoriteBeersSubscription.unsubscribe();
    }
  }
}
