import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { BeersService } from './beers.service';
import { Beer } from './beer.model';
import { PagedList } from '../shared/paged-list';
import { Subscription } from 'rxjs';
import { BeersParams } from './beers-params';
import { LoadingSpinnerComponent } from '../loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../shared-components/error-message/error-message.component';

@Component({
  selector: 'app-beers',
  standalone: true,
  imports: [LoadingSpinnerComponent, ErrorMessageComponent],
  templateUrl: './beers.component.html',
  styleUrl: './beers.component.css'
})
export class BeersComponent implements OnInit, OnDestroy {
  private beersService: BeersService = inject(BeersService);

  beers: PagedList<Beer> | undefined;
  error = '';
  loading = true;
  getBeersSubscription!: Subscription;

  ngOnInit(): void {
    this.getBeersSubscription = this.beersService
      .getBeers(new BeersParams(25, 1, 'name', 1))
      .subscribe({
        next: (beers: PagedList<Beer>) => {
          this.loading = true;
          this.beers = beers;
          this.loading = false;
        },
        error: () => {
          this.error = 'An error occurred while loading the beers';
          this.loading = false;
        }
      });
  }
  ngOnDestroy(): void {
    this.getBeersSubscription.unsubscribe();
  }
}
