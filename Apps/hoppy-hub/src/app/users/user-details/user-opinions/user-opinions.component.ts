import {
  Component,
  ElementRef,
  inject,
  Input,
  OnChanges,
  OnDestroy,
  ViewChild
} from '@angular/core';
import { User } from '../../user.model';
import { forkJoin, map, of, Subscription, switchMap } from 'rxjs';
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
import { UserOpinionsFiltersComponent } from './user-opinions-filters/user-opinions-filters.component';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';

@Component({
  selector: 'app-user-opinions',
  standalone: true,
  imports: [
    PaginationComponent,
    FormsModule,
    OpinionComponent,
    UserOpinionsFiltersComponent,
    LoadingSpinnerComponent,
    ErrorMessageComponent
  ],
  templateUrl: './user-opinions.component.html'
})
export class UserOpinionsComponent
  extends DataHelper
  implements OnChanges, OnDestroy
{
  @ViewChild('topSection') topSection!: ElementRef;
  @Input({ required: true }) user!: User;

  private opinionsService: OpinionsService = inject(OpinionsService);
  private beersService: BeersService = inject(BeersService);
  private opinionsParamsSubscription!: Subscription;
  private getOpinionsSubscription!: Subscription;

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
    this.opinionsParamsSubscription =
      this.opinionsService.paramsChanged.subscribe((params: OpinionsParams) => {
        this.opinionsParams = params;
        this.opinionsParams.userId = this.user.id;
        this.getUserOpinions();
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

          if (opinions.items.length === 0) {
            return of([]);
          }

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
