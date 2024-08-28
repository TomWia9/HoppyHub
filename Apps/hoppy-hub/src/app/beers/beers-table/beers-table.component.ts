import { Component, OnDestroy, OnInit, inject } from '@angular/core';
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
export class BeersTableComponent implements OnInit, OnDestroy {
  private beersService: BeersService = inject(BeersService);

  beersParams = new BeersParams(10, 1, 'ReleaseDate', 1);
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
          this.paginationData = this.getPaginationData();
          this.error = '';
          this.loading = false;
        },
        error: error => {
          this.error = 'An error occurred while loading the beers';

          if (error.error && error.error.errors) {
            const errorMessage = this.getErrorMessage(error.error.errors);
            this.error += errorMessage;
          }

          this.loading = false;
        }
      });
  }

  private getPaginationData(): Pagination {
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

  private getErrorMessage(array: { [key: string]: string }[]): string {
    if (array.length === 0) {
      return '';
    }

    const firstObject = Object.values(array)[0];
    const errorMessage = Object.values(firstObject)[0];

    if (!errorMessage) {
      return '';
    }

    return ': ' + errorMessage;
  }

  ngOnDestroy(): void {
    this.getBeersSubscription.unsubscribe();
    this.beersParamsSubscription.unsubscribe();
  }
}
