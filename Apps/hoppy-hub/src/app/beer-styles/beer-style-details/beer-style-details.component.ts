import {
  Component,
  ElementRef,
  inject,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { BeerStyle } from '../beer-style.model';
import { DataHelper } from '../../shared/data-helper';
import { ActivatedRoute } from '@angular/router';
import { Subject, Subscription, map, switchMap, takeUntil, tap } from 'rxjs';
import { Beer } from '../../beers/beer.model';
import { BeersParams } from '../../beers/beers-params';
import { BeersService } from '../../beers/beers.service';
import { PagedList } from '../../shared/paged-list';
import { Pagination } from '../../shared/pagination';
import { BeerStylesService } from '../beer-styles.service';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { BeerCardComponent } from '../../beers/beer-card/beer-card.component';
import { PaginationComponent } from '../../shared-components/pagination/pagination.component';
import { BeerStyleBeersFiltersComponent } from './beer-style-beers-filters/beer-style-beers-filters.component';

@Component({
  selector: 'app-beer-style-details',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    BeerCardComponent,
    PaginationComponent,
    BeerStyleBeersFiltersComponent
  ],
  templateUrl: './beer-style-details.component.html'
})
export class BeerStyleDetailsComponent
  extends DataHelper
  implements OnInit, OnDestroy
{
  @ViewChild('details') details!: ElementRef;

  private route: ActivatedRoute = inject(ActivatedRoute);
  private beerStylesService: BeerStylesService = inject(BeerStylesService);
  private beersService: BeersService = inject(BeersService);
  private destroy$ = new Subject<void>();

  beerStyle!: BeerStyle;
  error = '';
  beerStyleLoading = true;
  beerStyleBeersLoading = true;
  routeSubscription!: Subscription;
  beerStyleSubscription!: Subscription;
  beersSubscription!: Subscription;
  beersParamsSubscription!: Subscription;
  beersParams = new BeersParams({
    pageSize: 6,
    pageNumber: 1,
    sortBy: 'opinionsCount',
    sortDirection: 1
  });
  beers: PagedList<Beer> | undefined;
  paginationData!: Pagination;

  ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroy$),
        map(params => params.get('id')),
        tap(beerStyleId => this.refreshBeerStyleBeers(beerStyleId as string)),
        switchMap(beerStyleId =>
          this.beerStylesService.getBeerStyleById(beerStyleId as string).pipe(
            tap({
              next: (beerStyle: BeerStyle) => {
                this.beerStyle = beerStyle;
                this.error = '';
                this.scrollToDetails(-350);
              },
              error: () => {
                this.error = 'An error occurred while loading the beer style';
              },
              complete: () => {
                this.beerStyleLoading = false;
              }
            })
          )
        )
      )
      .subscribe();

    this.beersService.paramsChanged
      .pipe(
        takeUntil(this.destroy$),
        tap(params => (this.beersParams = params)),
        tap(() => (this.beerStyleBeersLoading = true)),
        switchMap(params =>
          this.beersService.getBeers(params).pipe(
            tap({
              next: (beers: PagedList<Beer>) => {
                this.beers = beers;
                this.paginationData = this.getPaginationData(beers);
                this.error = '';
              },
              error: () => {
                this.error = 'An error occurred while loading the beers';
              },
              complete: () => {
                this.beerStyleBeersLoading = false;
              }
            })
          )
        )
      )
      .subscribe();
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

  private refreshBeerStyleBeers(beerStyleId: string): void {
    this.beersParams.beerStyleId = beerStyleId;
    this.beersParams.pageNumber = 1;
    this.beersParams.searchQuery = '';
    this.beersParams.sortBy = 'ReleaseDate';
    this.beersParams.sortDirection = 1;
    this.beersService.paramsChanged.next(this.beersParams);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
