import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { BreweriesService } from '../breweries.service';
import { BreweriesParams } from '../breweries-params';
import { Brewery } from '../brewery.model';
import { PagedList } from '../../shared/paged-list';
import { Subscription } from 'rxjs';
import { PaginationComponent } from '../../shared-components/pagination/pagination.component';
import { Pagination } from '../../shared/pagination';
import { BreweriesTableFiltersComponent } from './breweries-table-filters/breweries-table-filters.component';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-breweries-table',
  standalone: true,
  templateUrl: './breweries-table.component.html',
  imports: [
    RouterModule,
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    PaginationComponent,
    BreweriesTableFiltersComponent
  ]
})
export class BreweriesTableComponent implements OnInit, OnDestroy {
  private breweriesService: BreweriesService = inject(BreweriesService);

  breweriesParams = new BreweriesParams(25, 1, 'Name', 1);
  breweries: PagedList<Brewery> | undefined;
  paginationData!: Pagination;
  error = '';
  loading = true;
  breweriesParamsSubscription!: Subscription;
  getBreweriesSubscription!: Subscription;

  ngOnInit(): void {
    this.breweriesParamsSubscription =
      this.breweriesService.paramsChanged.subscribe(
        (params: BreweriesParams) => {
          this.breweriesParams = params;
          this.getBreweries();
        }
      );
  }

  private getBreweries(): void {
    this.getBreweriesSubscription = this.breweriesService
      .getBreweries(this.breweriesParams)
      .subscribe({
        next: (breweries: PagedList<Brewery>) => {
          this.loading = true;
          this.breweries = breweries;
          this.paginationData = this.getPaginationData();
          this.error = '';
          this.loading = false;
        },
        error: error => {
          this.error = 'An error occurred while loading the breweries';

          if (error.error && error.error.errors) {
            const errorMessage = this.getErrorMessage(error.error.errors);
            this.error += errorMessage;
          }

          this.loading = false;
        }
      });
  }

  private getPaginationData(): Pagination {
    if (this.breweries) {
      return {
        CurrentPage: this.breweries.CurrentPage,
        HasNext: this.breweries.HasNext,
        HasPrevious: this.breweries.HasPrevious,
        TotalPages: this.breweries.TotalPages,
        TotalCount: this.breweries.TotalCount
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
    this.getBreweriesSubscription.unsubscribe();
    this.breweriesParamsSubscription.unsubscribe();
  }
}
