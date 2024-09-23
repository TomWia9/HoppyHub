import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  inject
} from '@angular/core';
import { BeersService } from '../beers.service';
import { Beer } from '../beer.model';
import { PagedList } from '../../shared/paged-list';
import { Subscription } from 'rxjs';
import { BeersParams } from '../beers-params';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { PaginationComponent } from '../../shared-components/pagination/pagination.component';
import { Pagination } from '../../shared/pagination';
import { BeersTableFiltersComponent } from './beers-table-filters/beers-table-filters.component';
import { RouterModule } from '@angular/router';
import { DataHelper } from '../../shared/data-helper';

@Component({
  selector: 'app-beers-table',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    PaginationComponent,
    BeersTableFiltersComponent,
    RouterModule
  ],
  templateUrl: './beers-table.component.html'
})
export class BeersTableComponent
  extends DataHelper
  implements OnInit, OnDestroy
{
  @ViewChild('topSection') topSection!: ElementRef;
  private beersService: BeersService = inject(BeersService);

  beersParams = new BeersParams({
    pageSize: 10,
    pageNumber: 1,
    sortBy: 'releaseDate',
    sortDirection: 1
  });
  beers: PagedList<Beer> | undefined;
  paginationData!: Pagination;
  error = '';
  loading = true;
  beersParamsSubscription!: Subscription;
  getBeersSubscription!: Subscription;

  ngOnInit(): void {
    this.beersService.paramsChanged.next(this.beersParams);
    this.beersParamsSubscription = this.beersService.paramsChanged.subscribe(
      (params: BeersParams) => {
        this.beersParams = params;
        this.getBeers();
      }
    );
  }

  private getBeers(): void {
    this.getBeersSubscription = this.beersService
      .getBeers(this.beersParams)
      .subscribe({
        next: (beers: PagedList<Beer>) => {
          this.loading = true;
          this.beers = beers;
          this.paginationData = this.getPaginationData(beers);
          this.error = '';
          this.loading = false;
        },
        error: error => {
          this.error = 'An error occurred while loading the beers';

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
    if (this.getBeersSubscription) {
      this.getBeersSubscription.unsubscribe();
    }
    if (this.beersParamsSubscription) {
      this.beersParamsSubscription.unsubscribe();
    }
  }
}
