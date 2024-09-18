import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { Subscription } from 'rxjs';
import { PagedList } from '../../shared/paged-list';
import { BeersService } from '../../beers/beers.service';
import { Beer } from '../../beers/beer.model';
import { BeersParams } from '../../beers/beers-params';
import { RecentBeerComponent } from './recent-beer/recent-beer.component';

@Component({
  selector: 'app-recent-beers',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    RecentBeerComponent
  ],
  templateUrl: './recent-beers.component.html'
})
export class RecentBeersComponent implements OnInit, OnDestroy {
  private beersService: BeersService = inject(BeersService);

  beers: PagedList<Beer> | undefined;
  error = '';
  loading = true;
  getBeersSubscription!: Subscription;

  ngOnInit(): void {
    this.getBeersSubscription = this.beersService
      .getBeers(
        new BeersParams({
          pageSize: 20,
          pageNumber: 1,
          sortBy: 'releaseDate',
          sortDirection: 1
        })
      )
      .subscribe({
        next: (beers: PagedList<Beer>) => {
          this.loading = true;
          this.beers = beers;
          this.error = '';
          this.loading = false;
        },
        error: () => {
          this.error = 'An error occurred while loading the recent beers';
          this.loading = false;
        }
      });
  }

  ngOnDestroy(): void {
    this.getBeersSubscription.unsubscribe();
  }
}
