import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { BeerStylesTableComponent } from '../../beer-styles/beer-styles-table/beer-styles-table.component';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterModule
} from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import { NewBeerStyleComponent } from './new-beer-style/new-beer-style.component';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-beer-style-management',
  standalone: true,
  imports: [BeerStylesTableComponent, RouterModule, FontAwesomeModule],
  templateUrl: './beer-style-management.component.html'
})
export class BeerStyleManagementComponent implements OnInit, OnDestroy {
  private router: Router = inject(Router);
  private route: ActivatedRoute = inject(ActivatedRoute);

  beerStyleSelected: boolean = false;
  newBeerStyle: boolean = false;
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
      this.newBeerStyle = childRoute.component === NewBeerStyleComponent;

      childRoute.paramMap.subscribe(paramMap => {
        this.beerStyleSelected = paramMap.has('id');
      });
    } else {
      this.beerStyleSelected = false;
      this.newBeerStyle = false;
    }
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
