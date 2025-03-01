import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { OpinionComponent } from '../../opinions/opinion/opinion.component';
import { Subscription } from 'rxjs';
import { Opinion } from '../../opinions/opinion.model';
import { OpinionsParams } from '../../opinions/opinions-params';
import { OpinionsService } from '../../opinions/opinions.service';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { PagedList } from '../../shared/paged-list';

@Component({
  selector: 'app-recent-opinions',
  standalone: true,
  templateUrl: './recent-opinions.component.html',
  imports: [OpinionComponent, LoadingSpinnerComponent, ErrorMessageComponent]
})
export class RecentOpinionsComponent implements OnInit, OnDestroy {
  private opinionsService: OpinionsService = inject(OpinionsService);

  opinions: Opinion[] = [];
  error = '';
  loading = true;
  getOpinionsSubscription!: Subscription;

  ngOnInit(): void {
    this.getOpinionsSubscription = this.opinionsService
      .getOpinions(
        new OpinionsParams({
          pageSize: 10,
          pageNumber: 1,
          sortBy: 'lastModified',
          sortDirection: 1,
          hasComment: true
        })
      )
      .subscribe({
        next: (opinions: PagedList<Opinion>) => {
          this.opinions = opinions.items;
          this.error = '';
          this.loading = false;
        },
        error: () => {
          this.error = 'An error occurred while loading the recent opinions';
          this.loading = false;
        }
      });
  }

  ngOnDestroy(): void {
    if (this.getOpinionsSubscription) {
      this.getOpinionsSubscription.unsubscribe();
    }
  }
}
