import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterOutlet
} from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import { filter, Subscription } from 'rxjs';

@Component({
  selector: 'app-beer-styles',
  standalone: true,
  imports: [RouterOutlet, FontAwesomeModule],
  templateUrl: './beer-styles.component.html'
})
export class BeerStylesComponent implements OnInit, OnDestroy {
  beerStyleSelected: boolean = false;
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
        this.beerStyleSelected = paramMap.has('id');
      });
    } else {
      this.beerStyleSelected = false;
    }
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
