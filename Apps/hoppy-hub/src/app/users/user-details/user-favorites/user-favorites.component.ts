import {
  Component,
  ElementRef,
  inject,
  Input,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { FavoritesService } from '../../../favorites/favorites.service';
import { FavoritesParams } from '../../../favorites/favorites-params';
import { Beer } from '../../../beers/beer.model';
import { switchMap, forkJoin, of, Subject, takeUntil, tap } from 'rxjs';
import { BeersService } from '../../../beers/beers.service';
import { DataHelper } from '../../../shared/data-helper';
import { PagedList } from '../../../shared/paged-list';
import { Pagination } from '../../../shared/pagination';
import { User } from '../../user.model';
import { BeerCardComponent } from '../../../beers/beer-card/beer-card.component';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { UserFavoritesFiltersComponent } from './user-favorites-filters/user-favorites-filters.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';

@Component({
  selector: 'app-user-favorites',
  standalone: true,
  imports: [
    BeerCardComponent,
    PaginationComponent,
    LoadingSpinnerComponent,
    UserFavoritesFiltersComponent,
    ErrorMessageComponent
  ],
  templateUrl: './user-favorites.component.html'
})
export class UserFavoritesComponent
  extends DataHelper
  implements OnInit, OnDestroy
{
  @ViewChild('topSection') topSection!: ElementRef;
  @Input({ required: true }) user!: User;
  @Input({ required: true }) editAccess: boolean = false;

  private favoritesService: FavoritesService = inject(FavoritesService);
  private beersService: BeersService = inject(BeersService);
  private destroy$ = new Subject<void>();

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

  ngOnInit(): void {
    this.refreshFavoriteBeers();
    this.favoritesService.paramsChanged
      .pipe(
        takeUntil(this.destroy$),
        tap(params => {
          this.favoriteBeersParams = params;
          this.favoriteBeersParams.userId = this.user.id;
        }),
        switchMap(() =>
          this.favoritesService.getFavorites(this.favoriteBeersParams).pipe(
            switchMap((favoriteBeers: PagedList<Beer>) => {
              this.paginationData = this.getPaginationData(favoriteBeers);

              if (favoriteBeers.items.length === 0) {
                return of([]);
              }

              return forkJoin(
                favoriteBeers.items.map(favoriteBeer =>
                  this.beersService.getBeerById(favoriteBeer.id)
                )
              );
            })
          )
        ),
        tap({
          next: (favoriteBeers: Beer[]) => {
            this.favoriteBeers = favoriteBeers;
            this.error = '';
            this.loading = false;
          },
          error: error => {
            this.error =
              'An error occurred while loading the user favorite beers';
            if (error.error?.errors) {
              this.error += this.getValidationErrorMessage(error.error.errors);
            }
            this.loading = false;
          }
        })
      )
      .subscribe();
  }

  refreshFavoriteBeers(): void {
    this.favoriteBeersParams.userId = this.user.id;
    this.favoritesService.paramsChanged.next(this.favoriteBeersParams);
  }

  scrollToTop(): void {
    const elementPosition =
      this.topSection.nativeElement.getBoundingClientRect().top +
      window.scrollY;

    window.scrollTo({
      top: elementPosition,
      behavior: 'smooth'
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
