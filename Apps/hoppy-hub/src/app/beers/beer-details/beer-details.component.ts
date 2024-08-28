import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Subscription, map } from 'rxjs';
import { Beer } from '../beer.model';
import { BeersService } from '../beers.service';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-beer-details',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    RouterModule,
    CommonModule
  ],
  templateUrl: './beer-details.component.html'
})
export class BeerDetailsComponent implements OnInit, OnDestroy {
  beer!: Beer;
  error = '';
  loading = true;
  routeSubscription!: Subscription;
  beerSubscription!: Subscription;

  private route: ActivatedRoute = inject(ActivatedRoute);
  private beersService: BeersService = inject(BeersService);

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(beerId => {
        this.beerSubscription = this.beersService
          .getBeerById(beerId as string)
          .subscribe({
            next: (beer: Beer) => {
              this.loading = true;
              this.beer = beer;
              this.error = '';
              this.loading = false;
            },
            error: () => {
              this.error = 'An error occurred while loading the beer';
              this.loading = false;
            }
          });
      });
  }

  ngOnDestroy(): void {
    if (this.beerSubscription) {
      this.beerSubscription.unsubscribe();
    }

    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
