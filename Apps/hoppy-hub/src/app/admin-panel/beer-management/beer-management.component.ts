import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { BeersFiltersModalComponent } from '../../beers/beers-table/beers-table-filters/beers-filters-modal/beers-filters-modal.component';
import { BeersTableComponent } from '../../beers/beers-table/beers-table.component';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterModule
} from '@angular/router';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { Subscription, filter } from 'rxjs';
import { NewBeerComponent } from './new-beer/new-beer.component';

@Component({
  selector: 'app-beer-management',
  standalone: true,
  imports: [
    BeersFiltersModalComponent,
    BeersTableComponent,
    RouterModule,
    FontAwesomeModule
  ],
  templateUrl: './beer-management.component.html'
})
export class BeerManagementComponent implements OnInit, OnDestroy {
  beerSelected: boolean = false;
  newBeer: boolean = false;
  routeSubscription!: Subscription;
  faInfoCircle = faInfoCircle;

  private router: Router = inject(Router);
  private route: ActivatedRoute = inject(ActivatedRoute);

  ngOnInit(): void {
    this.checkRoute();

    this.routeSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.checkRoute();
      });
  }

  private checkRoute(): void {
    const childRoute = this.route.firstChild;

    if (childRoute) {
      this.newBeer = childRoute.component === NewBeerComponent;

      childRoute.paramMap.subscribe(paramMap => {
        this.beerSelected = paramMap.has('id');
      });
    } else {
      this.beerSelected = false;
      this.newBeer = false;
    }
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
