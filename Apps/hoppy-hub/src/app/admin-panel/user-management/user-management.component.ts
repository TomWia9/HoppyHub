import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { UsersListComponent } from './users-list/users-list.component';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterModule
} from '@angular/router';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import { Subscription, filter } from 'rxjs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [UsersListComponent, RouterModule, FontAwesomeModule],
  templateUrl: './user-management.component.html'
})
export class UserManagementComponent implements OnInit, OnDestroy {
  private router: Router = inject(Router);
  private route: ActivatedRoute = inject(ActivatedRoute);

  userSelected: boolean = false;
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
      childRoute.paramMap.subscribe(paramMap => {
        this.userSelected = paramMap.has('id');
      });
    } else {
      this.userSelected = false;
    }
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
