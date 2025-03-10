import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterOutlet
} from '@angular/router';
import { BeersTableComponent } from './beers-table/beers-table.component';
import { BeersFiltersModalComponent } from './beers-table/beers-table-filters/beers-filters-modal/beers-filters-modal.component';
import { Subscription, filter } from 'rxjs';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-beers',
  standalone: true,
  imports: [
    RouterOutlet,
    BeersTableComponent,
    BeersFiltersModalComponent,
    FontAwesomeModule
  ],
  templateUrl: './beers.component.html'
})
export class BeersComponent implements OnInit, OnDestroy {
  beerSelected: boolean = false;
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
      childRoute.paramMap.subscribe(paramMap => {
        this.beerSelected = paramMap.has('id');
      });
    } else {
      this.beerSelected = false;
    }
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
