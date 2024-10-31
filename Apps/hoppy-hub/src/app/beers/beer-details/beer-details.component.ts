import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Subscription, map } from 'rxjs';
import { Beer } from '../beer.model';
import { BeersService } from '../beers.service';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { CommonModule } from '@angular/common';
import { BeerOpinionsComponent } from './beer-opinions/beer-opinions.component';
import { AuthService } from '../../auth/auth.service';
import { AuthUser } from '../../auth/auth-user.model';
import { FavoriteComponent } from './favorite/favorite.component';

@Component({
  selector: 'app-beer-details',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    RouterModule,
    CommonModule,
    BeerOpinionsComponent,
    FavoriteComponent
  ],
  templateUrl: './beer-details.component.html'
})
export class BeerDetailsComponent implements OnInit, OnDestroy {
  beer!: Beer;
  error = '';
  loading = true;
  user: AuthUser | null = null;
  routeSubscription!: Subscription;
  beerSubscription!: Subscription;
  userSubscription!: Subscription;

  private route: ActivatedRoute = inject(ActivatedRoute);
  private beersService: BeersService = inject(BeersService);
  private authService: AuthService = inject(AuthService);

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
              this.userSubscription = this.authService.user.subscribe(
                (user: AuthUser | null) => {
                  this.user = user;
                  this.loading = false;
                }
              );
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
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }
}
