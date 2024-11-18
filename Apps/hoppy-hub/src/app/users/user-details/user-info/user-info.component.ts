import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { map, Subscription } from 'rxjs';
import { FavoritesService } from '../../../favorites/favorites.service';
import { FavoritesParams } from '../../../favorites/favorites-params';
import { PagedList } from '../../../shared/paged-list';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { Beer } from '../../../beers/beer.model';
import { OpinionsService } from '../../../opinions/opinions.service';
import { OpinionsParams } from '../../../opinions/opinions-params';
import { Opinion } from '../../../opinions/opinion.model';
import { faBeerMugEmpty } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faHeart } from '@fortawesome/free-regular-svg-icons';
import { UsersService } from '../../users.service';
import { User } from '../../user.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-info',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    FontAwesomeModule,
    CommonModule
  ],
  templateUrl: './user-info.component.html'
})
export class UserInfoComponent implements OnInit, OnDestroy {
  routeSubscription!: Subscription;
  usersSubscription!: Subscription;
  favoritesSubscription!: Subscription;
  opinionsSubscription!: Subscription;
  user!: User;
  userFavoritesCount: number = 0;
  userBeersRatedCount: number = 0;
  error = '';
  loading = true;
  faBeerMugEmpty = faBeerMugEmpty;
  faHeart = faHeart;

  private route: ActivatedRoute = inject(ActivatedRoute);
  private usersService: UsersService = inject(UsersService);
  private favoritesService: FavoritesService = inject(FavoritesService);
  private opinionsService: OpinionsService = inject(OpinionsService);

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(userId => {
        this.usersSubscription = this.usersService
          .getUserById(userId as string)
          .subscribe({
            next: (user: User) => {
              this.user = user;
              this.error = '';
              this.loading = false;
            },
            error: () => {
              this.error = 'An error occurred while loading the user';
              this.loading = false;
            }
          });
        this.favoritesSubscription = this.favoritesService
          .getFavorites(new FavoritesParams({ userId: userId as string }))
          .subscribe({
            next: (favoritesList: PagedList<Beer>) => {
              this.userFavoritesCount = favoritesList.TotalCount;
              this.error = '';
              this.loading = false;
            },
            error: () => {
              this.error =
                'An error occurred while loading the user favorites count';
              this.loading = false;
            }
          });
        this.opinionsSubscription = this.opinionsService
          .getOpinions(new OpinionsParams({ userId: userId as string }))
          .subscribe({
            next: (opinionsList: PagedList<Opinion>) => {
              this.userBeersRatedCount = opinionsList.TotalCount;
              this.error = '';
              this.loading = false;
            },
            error: () => {
              this.error =
                'An error occurred while loading the user beers rated count';
              this.loading = false;
            }
          });
      });
  }
  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
    if (this.usersSubscription) {
      this.usersSubscription.unsubscribe();
    }
    if (this.favoritesSubscription) {
      this.favoritesSubscription.unsubscribe();
    }
    if (this.opinionsSubscription) {
      this.opinionsSubscription.unsubscribe();
    }
  }
}
