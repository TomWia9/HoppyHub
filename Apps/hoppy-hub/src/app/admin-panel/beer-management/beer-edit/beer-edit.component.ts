import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subscription, map } from 'rxjs';
import { Beer } from '../../../beers/beer.model';
import { BeersService } from '../../../beers/beers.service';

@Component({
  selector: 'app-beer-edit',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    RouterModule,
    CommonModule
  ],
  templateUrl: './beer-edit.component.html'
})
export class BeerEditComponent implements OnInit, OnDestroy {
  beer!: Beer;
  error = '';
  loading = true;
  routeSubscription!: Subscription;
  beerSubscription!: Subscription;

  private route: ActivatedRoute = inject(ActivatedRoute);
  private beersService: BeersService = inject(BeersService);

  ngOnInit(): void {
    this.getBeer();
  }

  getBeer(): void {
    this.loading = true;
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(beerId => {
        this.beerSubscription = this.beersService
          .getBeerById(beerId as string)
          .subscribe({
            next: (beer: Beer) => {
              this.beer = beer;
              this.error = '';
              window.scrollTo({ top: 0, behavior: 'smooth' });
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
