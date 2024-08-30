import {
  Component,
  inject,
  Input,
  OnChanges,
  OnDestroy,
  OnInit
} from '@angular/core';
import { OpinionsService } from '../../../opinions/opinions.service';
import { OpinionsParams } from '../../../opinions/opinions-params';
import { Opinion } from '../../../opinions/opinion.model';
import { Subscription } from 'rxjs';
import { PagedList } from '../../../shared/paged-list';
import { Pagination } from '../../../shared/pagination';
import { Beer } from '../../beer.model';
import { OpinionComponent } from '../../../opinions/opinion/opinion.component';

@Component({
  selector: 'app-beer-opinions',
  standalone: true,
  imports: [OpinionComponent],
  templateUrl: './beer-opinions.component.html'
})
export class BeerOpinionsComponent implements OnInit, OnChanges, OnDestroy {
  @Input({ required: true }) beer!: Beer;

  private opinionsService: OpinionsService = inject(OpinionsService);

  opinionsParams = new OpinionsParams(10, 1, 'created', 1);
  opinions: PagedList<Opinion> | undefined;
  paginationData!: Pagination;
  error = '';
  loading = true;
  opinionsParamsSubscription!: Subscription;
  getOpinionsSubscription!: Subscription;

  ngOnInit(): void {
    this.opinionsParamsSubscription =
      this.opinionsService.paramsChanged.subscribe((params: OpinionsParams) => {
        this.opinionsParams = params;
        this.getOpinions();
      });
  }

  ngOnChanges() {
    this.opinionsParams.beerId = this.beer.id;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
  }

  private getOpinions(): void {
    this.getOpinionsSubscription = this.opinionsService
      .getOpinions(this.opinionsParams)
      .subscribe({
        next: (opinions: PagedList<Opinion>) => {
          this.loading = true;
          this.opinions = opinions;
          this.paginationData = this.getPaginationData();
          this.error = '';
          this.loading = false;
        },
        error: error => {
          this.error = 'An error occurred while loading the opinions';

          if (error.error && error.error.errors) {
            const errorMessage = this.getErrorMessage(error.error.errors);
            this.error += errorMessage;
          }

          this.loading = false;
        }
      });
  }

  private getPaginationData(): Pagination {
    if (this.opinions) {
      return {
        CurrentPage: this.opinions.CurrentPage,
        HasNext: this.opinions.HasNext,
        HasPrevious: this.opinions.HasPrevious,
        TotalPages: this.opinions.TotalPages,
        TotalCount: this.opinions.TotalCount
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
    this.getOpinionsSubscription.unsubscribe();
    this.opinionsParamsSubscription.unsubscribe();
  }
}
