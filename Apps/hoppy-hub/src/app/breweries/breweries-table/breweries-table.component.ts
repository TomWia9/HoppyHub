import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { BreweriesService } from '../breweries.service';
import { BreweriesParams } from '../breweries-params';
import { Brewery } from '../brewery.model';
import { PagedList } from '../../shared/paged-list';
import { Subject, takeUntil, tap } from 'rxjs';
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
  private destroy$ = new Subject<void>();

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

  ngOnInit(): void {
    this.breweriesService.paramsChanged
      .pipe(
        takeUntil(this.destroy$),
        tap((params: BreweriesParams) => {
          this.breweriesParams = params;
          this.getBreweries();
        })
      )
      .subscribe();
  }

  private getBreweries(): void {
    this.breweriesService
      .getBreweries(this.breweriesParams)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: (breweries: PagedList<Brewery>) => {
            this.breweries = breweries;
            this.paginationData = this.getPaginationData(breweries);
            this.error = '';
          },
          error: error => {
            this.error = 'An error occurred while loading the breweries';

            if (error.error && error.error.errors) {
              const errorMessage = this.getValidationErrorMessage(
                error.error.errors
              );
              this.error += errorMessage;
            }
          },
          complete: () => {
            this.loading = false;
          }
        })
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
