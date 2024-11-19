import { Component, inject, Input, OnChanges, OnDestroy } from '@angular/core';
import { User } from '../../user.model';
import { forkJoin, map, Subscription, switchMap } from 'rxjs';
import { Opinion } from '../../../opinions/opinion.model';
import { OpinionsParams } from '../../../opinions/opinions-params';
import { OpinionsService } from '../../../opinions/opinions.service';
import { PagedList } from '../../../shared/paged-list';
import { DataHelper } from '../../../shared/data-helper';
import { Pagination } from '../../../shared/pagination';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { FormsModule } from '@angular/forms';
import { OpinionComponent } from '../../../opinions/opinion/opinion.component';
import { Beer } from '../../../beers/beer.model';
import { BeersService } from '../../../beers/beers.service';

@Component({
  selector: 'app-user-opinions',
  standalone: true,
  imports: [PaginationComponent, FormsModule, OpinionComponent],
  templateUrl: './user-opinions.component.html'
})
export class UserOpinionsComponent
  extends DataHelper
  implements OnChanges, OnDestroy
{
  @Input({ required: true }) user!: User;

  private opinionsService: OpinionsService = inject(OpinionsService);
  private beersService: BeersService = inject(BeersService);
  private opinionsParamsSubscription!: Subscription;
  private getOpinionsSubscription!: Subscription;

  sortOptions = OpinionsParams.sortOptions;
  selectedSortOptionIndex: number = 0;
  opinionsParams = new OpinionsParams({
    pageSize: 10,
    pageNumber: 1,
    sortBy: 'created',
    sortDirection: 1
  });
  opinionBeerPairs: { opinion: Opinion; beer: Beer }[] = [];
  paginationData!: Pagination;
  error = '';
  loading = true;

  ngOnChanges(): void {
    this.refreshOpinions();
  }

  refreshOpinions(): void {
    this.opinionsParams.userId = this.user.id;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
    this.opinionsService.paramsChanged.subscribe((params: OpinionsParams) => {
      this.opinionsParams = params;
      this.opinionsParams.userId = this.user.id;
      this.getUserOpinions();
    });
  }

  onSort(): void {
    this.opinionsParams.pageNumber = 1;
    this.opinionsParams.sortBy =
      this.sortOptions[this.selectedSortOptionIndex].value;
    this.opinionsParams.sortDirection =
      this.sortOptions[this.selectedSortOptionIndex].direction;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
  }

  onFiltersClear(): void {
    this.selectedSortOptionIndex = 0;
    this.opinionsParams = new OpinionsParams({
      pageSize: 10,
      pageNumber: 1,
      sortBy: 'created',
      sortDirection: 1
    });

    if (
      JSON.stringify(this.opinionsService.paramsChanged.value) !=
      JSON.stringify(this.opinionsParams)
    ) {
      this.opinionsService.paramsChanged.next(this.opinionsParams);
    }
  }

  private getUserOpinions(): void {
    this.getOpinionsSubscription = this.opinionsService
      .getOpinions(this.opinionsParams)
      .pipe(
        switchMap((opinions: PagedList<Opinion>) => {
          const beerRequests = opinions.items.map(opinion =>
            this.beersService
              .getBeerById(opinion.beerId)
              .pipe(map(beer => ({ opinion, beer })))
          );
          this.paginationData = this.getPaginationData(opinions);

          return forkJoin(beerRequests);
        })
      )
      .subscribe({
        next: (results: { opinion: Opinion; beer: Beer }[]) => {
          this.loading = true;
          this.opinionBeerPairs = results;
          this.error = '';
          this.loading = false;
        },
        error: error => {
          this.error = 'An error occurred while loading the user opinions';

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
    if (this.opinionsParamsSubscription) {
      this.opinionsParamsSubscription.unsubscribe();
    }
    if (this.getOpinionsSubscription) {
      this.getOpinionsSubscription.unsubscribe();
    }
  }
}
