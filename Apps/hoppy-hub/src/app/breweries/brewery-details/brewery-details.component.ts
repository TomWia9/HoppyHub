import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { ActivatedRoute } from '@angular/router';
import { Subscription, map } from 'rxjs';

@Component({
  selector: 'app-brewery-details',
  standalone: true,
  templateUrl: './brewery-details.component.html',
  imports: [LoadingSpinnerComponent, ErrorMessageComponent]
})
export class BreweryDetailsComponent implements OnInit, OnDestroy {
  breweryId: string | null = null;
  error = '';
  loading = false;
  routeSubscription!: Subscription;

  private route: ActivatedRoute = inject(ActivatedRoute);

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(breweryId => {
        this.breweryId = breweryId;
      });
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
