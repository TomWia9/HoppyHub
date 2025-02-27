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
import { Subject, switchMap, takeUntil, tap } from 'rxjs';
import { Opinion } from '../../../opinions/opinion.model';
import { OpinionsParams } from '../../../opinions/opinions-params';
import { OpinionsService } from '../../../opinions/opinions.service';
import { PagedList } from '../../../shared/paged-list';
import { DataHelper } from '../../../shared/data-helper';
import { Pagination } from '../../../shared/pagination';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { FormsModule } from '@angular/forms';
import { OpinionComponent } from '../../../opinions/opinion/opinion.component';
import { BeersService } from '../../../beers/beers.service';
import { UserOpinionsFiltersComponent } from './user-opinions-filters/user-opinions-filters.component';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { UpsertOpinionModalComponent } from '../../../opinions/upsert-opinion-modal/upsert-opinion-modal.component';
import { DeleteOpinionModalComponent } from '../../../opinions/delete-opinion-modal/delete-opinion-modal.component';

@Component({
  selector: 'app-user-opinions',
  standalone: true,
  imports: [
    PaginationComponent,
    FormsModule,
    OpinionComponent,
    UserOpinionsFiltersComponent,
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    UpsertOpinionModalComponent,
    DeleteOpinionModalComponent
  ],
  templateUrl: './user-opinions.component.html'
})
export class UserOpinionsComponent
  extends DataHelper
  implements OnChanges, OnDestroy
{
  @ViewChild('topSection') topSection!: ElementRef;
  @Input({ required: true }) user!: User;
  @Input({ required: true }) accountOwner: boolean = false;

  private opinionsService: OpinionsService = inject(OpinionsService);
  private beersService: BeersService = inject(BeersService);
  private destroy$ = new Subject<void>();

  opinionsParams = new OpinionsParams({
    pageSize: 10,
    pageNumber: 1,
    sortBy: 'created',
    sortDirection: 1
  });
  opinions: Opinion[] = [];
  paginationData!: Pagination;
  error = '';
  loading = true;

  ngOnChanges(): void {
    this.refreshOpinions();
    this.opinionsService.paramsChanged
      .pipe(
        takeUntil(this.destroy$),
        tap(params => {
          this.opinionsParams = params;
          this.opinionsParams.userId = this.user.id;
        }),
        switchMap(() => this.opinionsService.getOpinions(this.opinionsParams)),
        tap({
          next: (opinions: PagedList<Opinion>) => {
            this.paginationData = this.getPaginationData(opinions);
            this.opinions = opinions.items;
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
        })
      )
      .subscribe();
  }

  refreshOpinions(): void {
    this.opinionsParams.userId = this.user.id;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
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
    this.destroy$.next();
    this.destroy$.complete();
  }
}
