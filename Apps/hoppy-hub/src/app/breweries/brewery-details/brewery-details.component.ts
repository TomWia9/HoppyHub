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
import { Subject, map, switchMap, takeUntil, tap } from 'rxjs';
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
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroy$),
        map(params => params.get('id')),
        tap(breweryId => this.refreshBreweryBeers(breweryId as string)),
        switchMap(breweryId =>
          this.breweriesService.getBreweryById(breweryId as string).pipe(
            tap({
              next: (brewery: Brewery) => {
                this.brewery = brewery;
                this.error = '';
                this.scrollToDetails(-350);
              },
              error: () => {
                this.error = 'An error occurred while loading the brewery';
              },
              complete: () => {
                this.breweryLoading = false;
              }
            })
          )
        ),
        switchMap(brewery => {
          this.beersParams.breweryId = brewery.id;

          return this.beersService.paramsChanged.pipe(
            tap((params: BeersParams) => {
              this.beersParams = params;
              this.beersParams.breweryId = brewery.id;
              this.getBreweryBeers();
            })
          );
        })
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

  private getBreweryBeers(): void {
    this.beersService
      .getBeers(this.beersParams)
      .pipe(
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
            this.breweryBeersLoading = false;
          }
        })
      )
      .subscribe();
  }

  private refreshBreweryBeers(breweryId: string): void {
    this.beersParams.breweryId = breweryId;
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
