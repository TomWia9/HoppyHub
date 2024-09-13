import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Subscription, map } from 'rxjs';
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
  favorite = false;
  user: AuthUser | null | undefined;
  routeSubscription!: Subscription;
  beerSubscription!: Subscription;
  userSubscription!: Subscription;
  faStarr = faStar;

  private route: ActivatedRoute = inject(ActivatedRoute);
  private beersService: BeersService = inject(BeersService);
  private modalService: ModalService = inject(ModalService);
  private authService: AuthService = inject(AuthService);

  ngOnInit(): void {
    this.userSubscription = this.authService.user.subscribe(
      (user: AuthUser | null) => {
        this.user = user;
        //TODO: Check if user already added beer to favorites
      }
    );
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
      console.log('Add to favorites');
      //favoritesService.AddToFavorites
      this.favorite = true;
    } else {
      console.log('Remove from favorites');
      //favoritesService.RemoveFromFavorites
      this.favorite = false;
    }
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
