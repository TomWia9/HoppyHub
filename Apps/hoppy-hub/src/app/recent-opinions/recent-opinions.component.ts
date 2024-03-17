import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { RecentOpinionComponent } from './recent-opinion/recent-opinion.component';
import { OpinionsService } from '../opinions/opinions.service';
import { Subscription } from 'rxjs';
import { Opinion } from '../opinions/opinion.model';
import { PagedList } from '../shared/paged-list';
import { OpinionsParams } from '../opinions/opinions-params';
import { LoadingSpinnerComponent } from '../loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../shared-components/error-message/error-message.component';

@Component({
  selector: 'app-recent-opinions',
  standalone: true,
  templateUrl: './recent-opinions.component.html',
  imports: [
    RecentOpinionComponent,
    LoadingSpinnerComponent,
    ErrorMessageComponent
  ]
})
export class RecentOpinionsComponent implements OnInit, OnDestroy {
  private opinionsService: OpinionsService = inject(OpinionsService);

  opinions: PagedList<Opinion> | undefined;
  error = '';
  loading = true;
  getOpinionsSubscription!: Subscription;

  ngOnInit(): void {
    this.getOpinionsSubscription = this.opinionsService
      .getOpinions(new OpinionsParams(5, 1, 'lastModified', 1))
      .subscribe({
        next: (opinions: PagedList<Opinion>) => {
          this.loading = true;
          this.opinions = opinions;
          this.loading = false;
        },
        error: () => {
          this.error = 'An error occurred while loading the recent opinions';
          this.loading = false;
        }
      });
  }

  ngOnDestroy(): void {
    this.getOpinionsSubscription.unsubscribe();
  }
}
