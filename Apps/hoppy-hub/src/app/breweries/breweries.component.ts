import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { BreweriesTableComponent } from './breweries-table/breweries-table.component';
import { BreweriesFiltersModalComponent } from './breweries-table/breweries-table-filters/breweries-filters-modal/breweries-filters-modal.component';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterOutlet
} from '@angular/router';
import { Subscription, filter } from 'rxjs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-breweries',
  standalone: true,
  templateUrl: './breweries.component.html',
  imports: [
    RouterOutlet,
    BreweriesTableComponent,
    BreweriesFiltersModalComponent,
    FontAwesomeModule
  ]
})
export class BreweriesComponent implements OnInit, OnDestroy {
  brewerySelected: boolean = false;
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
        this.brewerySelected = paramMap.has('id');
      });
    } else {
      this.brewerySelected = false;
    }
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
