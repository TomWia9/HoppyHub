import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Subscription, map, take, tap } from 'rxjs';
import { Beer } from '../beer.model';
import { BeersService } from '../beers.service';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { CommonModule } from '@angular/common';
import { BeerOpinionsComponent } from './beer-opinions/beer-opinions.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faStar } from '@fortawesome/free-solid-svg-icons';
import { AuthService } from '../../auth/auth.service';
import { ModalService, ModalType } from '../../services/modal.service';
import { AuthUser } from '../../auth/auth-user.model';
import { FavoritesService } from '../../favorites/favorites.service';
import {
  AlertService,
  AlertType
} from '../../shared-components/alert/alert.service';
import { FavoritesParams } from '../../favorites/favorites-params';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-beer-details',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    RouterModule,
    CommonModule,
    BeerOpinionsComponent,
    FontAwesomeModule
  ],
  templateUrl: './beer-details.component.html'
})
export class BeerDetailsComponent implements OnInit, OnDestroy {
  beer!: Beer;
  error = '';
  loading = true;
  favoriteLoading = true;
  favorite = false;
  user: AuthUser | null = null;
  routeSubscription!: Subscription;
  beerSubscription!: Subscription;
  userSubscription!: Subscription;
  createFavoriteSubscription!: Subscription;
  deleteFavoriteSubscription!: Subscription;
  getUserFavoritesSubsciption!: Subscription;
  faStarr = faStar;

  private route: ActivatedRoute = inject(ActivatedRoute);
  private favoritesService: FavoritesService = inject(FavoritesService);
  private beersService: BeersService = inject(BeersService);
  private modalService: ModalService = inject(ModalService);
  private authService: AuthService = inject(AuthService);
  private alertService: AlertService = inject(AlertService);

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(beerId => {
        this.beerSubscription = this.beersService
          .getBeerById(beerId as string)
          .subscribe({
            next: (beer: Beer) => {
              this.loading = true;
              this.beer = beer;
              this.error = '';
              window.scrollTo({ top: 0, behavior: 'smooth' });
              this.loading = false;
              this.userSubscription = this.authService.user.subscribe(
                (user: AuthUser | null) => {
                  this.user = user;
                  this.checkIfUserAlreadyAddedBeerToFavorites();
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

  onToggleFavorite() {
    if (!this.user) {
      this.modalService.openModal(ModalType.Login);
    } else if (!this.favorite) {
      this.loading = true;
      this.createFavoriteSubscription = this.favoritesService
        .createFavorite(this.beer.id)
        .subscribe({
          next: () => {
            this.favorite = true;
            this.alertService.openAlert(
              AlertType.Success,
              'Successfully added to favorites'
            );
            this.loading = false;
          },
          error: error => {
            this.handleError(error);
          }
        });
    } else {
      this.loading = true;
      this.deleteFavoriteSubscription = this.favoritesService
        .deleteFavorite(this.beer.id)
        .subscribe({
          next: () => {
            this.favorite = false;
            this.alertService.openAlert(
              AlertType.Success,
              'Successfully removed from favorites'
            );
            this.loading = false;
          },
          error: error => {
            this.handleError(error);
          }
        });
    }
  }

  private checkIfUserAlreadyAddedBeerToFavorites() {
    this.favoriteLoading = true;
    if (!this.user) {
      this.favorite = false;
      this.favoriteLoading = false;
    } else {
      this.getUserFavoritesSubsciption = this.favoritesService
        .getFavorites(
          new FavoritesParams(
            1,
            1,
            undefined,
            undefined,
            undefined,
            this.beer.id,
            this.user.id
          )
        )
        .pipe(
          take(1),
          map(beers => (beers.TotalCount > 0 ? true : false)),
          tap(favorite => {
            console.log(favorite);

            this.favorite = favorite;
            this.favoriteLoading = false;
          })
        )
        .subscribe();
    }
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = null;

    if (error.error) {
      const firstKey = Object.keys(error.error?.errors)[0] ?? null;
      const firstValueArray = error.error?.errors[firstKey] as string[];
      errorMessage = firstValueArray[0];
    }

    if (!errorMessage) {
      this.alertService.openAlert(AlertType.Error, 'Something went wrong');
    } else {
      this.alertService.openAlert(AlertType.Error, errorMessage);
    }

    this.loading = false;
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
    if (this.createFavoriteSubscription) {
      this.createFavoriteSubscription.unsubscribe();
    }
    if (this.deleteFavoriteSubscription) {
      this.deleteFavoriteSubscription.unsubscribe();
    }
    if (this.getUserFavoritesSubsciption) {
      this.getUserFavoritesSubsciption.unsubscribe();
    }
  }
}
