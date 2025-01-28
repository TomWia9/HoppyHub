import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { BreweriesFiltersModalComponent } from '../../breweries/breweries-table/breweries-table-filters/breweries-filters-modal/breweries-filters-modal.component';
import { BreweriesTableComponent } from '../../breweries/breweries-table/breweries-table.component';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterModule
} from '@angular/router';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { filter, Subscription } from 'rxjs';
import { NewBreweryComponent } from './new-brewery/new-brewery.component';

@Component({
  selector: 'app-brewery-management',
  standalone: true,
  imports: [
    BreweriesFiltersModalComponent,
    BreweriesTableComponent,
    RouterModule,
    FontAwesomeModule
  ],
  templateUrl: './brewery-management.component.html'
})
export class BreweryManagementComponent implements OnInit, OnDestroy {
  private router: Router = inject(Router);
  private route: ActivatedRoute = inject(ActivatedRoute);

  brewerySelected: boolean = false;
  newBrewery: boolean = false;
  routeSubscription!: Subscription;
  faInfoCircle = faInfoCircle;

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
      this.newBrewery = childRoute.component === NewBreweryComponent;

      childRoute.paramMap.subscribe(paramMap => {
        this.brewerySelected = paramMap.has('id');
      });
    } else {
      this.brewerySelected = false;
      this.newBrewery = false;
    }
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
