import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { ActivatedRoute } from '@angular/router';
import { Subscription, map } from 'rxjs';
import { BreweriesService } from '../breweries.service';
import { Brewery } from '../brewery.model';

@Component({
  selector: 'app-brewery-details',
  standalone: true,
  templateUrl: './brewery-details.component.html',
  imports: [LoadingSpinnerComponent, ErrorMessageComponent]
})
export class BreweryDetailsComponent implements OnInit, OnDestroy {
  brewery!: Brewery;
  error = '';
  loading = true;
  routeSubscription!: Subscription;
  brewerySubscription!: Subscription;

  private route: ActivatedRoute = inject(ActivatedRoute);
  private breweriesService: BreweriesService = inject(BreweriesService);

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(breweryId => {
        this.brewerySubscription = this.breweriesService
          .getBreweryById(breweryId as string)
          .subscribe({
            next: (brewery: Brewery) => {
              this.loading = true;
              this.brewery = brewery;
              this.error = '';
              this.loading = false;
            },
            error: () => {
              this.error = 'An error occurred while loading the brewery';
              this.loading = false;
            }
          });
      });
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
    if (this.brewerySubscription) {
      this.brewerySubscription.unsubscribe();
    }
  }
}
