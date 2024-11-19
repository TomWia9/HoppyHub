import { Component, inject, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
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
  @Input({ required: true }) user!: User;

  private favoritesService: FavoritesService = inject(FavoritesService);
  private opinionsService: OpinionsService = inject(OpinionsService);
  private favoritesSubscription!: Subscription;
  private opinionsSubscription!: Subscription;

  userFavoritesCount: number = 0;
  userBeersRatedCount: number = 0;
  error = '';
  loading = true;
  faBeerMugEmpty = faBeerMugEmpty;
  faHeart = faHeart;

  ngOnInit(): void {
    this.favoritesSubscription = this.favoritesService
      .getFavorites(new FavoritesParams({ userId: this.user.id }))
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
      .getOpinions(new OpinionsParams({ userId: this.user.id }))
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
  }
  ngOnDestroy(): void {
    if (this.favoritesSubscription) {
      this.favoritesSubscription.unsubscribe();
    }
    if (this.opinionsSubscription) {
      this.opinionsSubscription.unsubscribe();
    }
  }
}
