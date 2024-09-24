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
import { DataHelper } from '../../shared/data-helper';

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
export class BreweriesTableComponent
  extends DataHelper
  implements OnInit, OnDestroy
{
  private breweriesService: BreweriesService = inject(BreweriesService);

  breweriesParams = new BreweriesParams({
    pageSize: 25,
    pageNumber: 1,
    sortBy: 'Name',
    sortDirection: 1
  });
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
          this.paginationData = this.getPaginationData(breweries);
          this.error = '';
          this.loading = false;
        },
        error: error => {
          this.error = 'An error occurred while loading the breweries';

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
    if (this.getBreweriesSubscription) {
      this.getBreweriesSubscription.unsubscribe();
    }
    if (this.breweriesParamsSubscription) {
      this.breweriesParamsSubscription.unsubscribe();
    }
  }
}
