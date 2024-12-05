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
import { Subscription, map } from 'rxjs';
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

@Component({
  selector: 'app-beer-style-details',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    BeerCardComponent,
    PaginationComponent
  ],
  templateUrl: './beer-style-details.component.html'
})
export class BeerStyleDetailsComponent
  extends DataHelper
  implements OnInit, OnDestroy
{
  @ViewChild('details') details!: ElementRef;

  beerStyle!: BeerStyle;
  error = '';
  beerStyleLoading = true;
  beerStyleBeersLoading = true;
  routeSubscription!: Subscription;
  beerStyleSubscription!: Subscription;
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
  private beerStylesService: BeerStylesService = inject(BeerStylesService);
  private beersService: BeersService = inject(BeersService);

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(beerStyleId => {
        this.resetBeerStyleDetails(beerStyleId as string);

        this.beerStyleSubscription = this.beerStylesService
          .getBeerStyleById(beerStyleId as string)
          .subscribe({
            next: (beerStyle: BeerStyle) => {
              this.beerStyleLoading = true;
              this.beerStyle = beerStyle;
              this.error = '';
              this.scrollToDetails(-350);
              this.beerStyleLoading = false;
            },
            error: () => {
              this.error = 'An error occurred while loading the beer style';
              this.beerStyleLoading = false;
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
          this.beerStyleBeersLoading = true;
          this.beers = beers;
          this.paginationData = this.getPaginationData(beers);
          this.error = '';
          this.beerStyleBeersLoading = false;
        },
        error: () => {
          this.error = 'An error occurred while loading the beers';
          this.beerStyleBeersLoading = false;
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

  private resetBeerStyleDetails(beerStyleId: string): void {
    if (this.beerStyleSubscription) {
      this.beerStyleSubscription.unsubscribe();
    }
    if (this.beersParamsSubscription) {
      this.beersParamsSubscription.unsubscribe();
    }
    if (this.beersSubscription) {
      this.beersSubscription.unsubscribe();
    }

    this.beersParams.beerStyleId = beerStyleId;
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
    this.resetBeerStyleDetails(this.beerStyle.id);

    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
