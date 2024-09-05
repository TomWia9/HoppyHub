import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { OpinionComponent } from '../../opinions/opinion/opinion.component';
import { forkJoin, map, Subscription, switchMap } from 'rxjs';
import { Opinion } from '../../opinions/opinion.model';
import { OpinionsParams } from '../../opinions/opinions-params';
import { OpinionsService } from '../../opinions/opinions.service';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { PagedList } from '../../shared/paged-list';
import { BeersService } from '../../beers/beers.service';
import { Beer } from '../../beers/beer.model';

@Component({
  selector: 'app-recent-opinions',
  standalone: true,
  templateUrl: './recent-opinions.component.html',
  imports: [OpinionComponent, LoadingSpinnerComponent, ErrorMessageComponent]
})
export class RecentOpinionsComponent implements OnInit, OnDestroy {
  private opinionsService: OpinionsService = inject(OpinionsService);
  private beersService: BeersService = inject(BeersService);

  opinionBeerPairs: { opinion: Opinion; beer: Beer }[] = [];
  error = '';
  loading = true;
  getOpinionsSubscription!: Subscription;

  ngOnInit(): void {
    this.getOpinionsSubscription = this.opinionsService
      .getOpinions(new OpinionsParams(5, 1, 'lastModified', 1))
      .pipe(
        switchMap((opinions: PagedList<Opinion>) => {
          const beerRequests = opinions.items.map(opinion =>
            this.beersService
              .getBeerById(opinion.beerId)
              .pipe(map(beer => ({ opinion, beer })))
          );
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
