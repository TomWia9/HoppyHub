import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { UserInfoComponent } from './user-info/user-info.component';
import { map, Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { User } from '../user.model';
import { UsersService } from '../users.service';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { UserOpinionsComponent } from './user-opinions/user-opinions.component';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [
    UserInfoComponent,
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    UserOpinionsComponent
  ],
  templateUrl: './user-details.component.html'
})
export class UserDetailsComponent implements OnInit, OnDestroy {
  private route: ActivatedRoute = inject(ActivatedRoute);
  private usersService: UsersService = inject(UsersService);
  private routeSubscription!: Subscription;
  private userSubscription!: Subscription;

  user!: User;
  error = '';
  loading = true;

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(userId => {
        this.userSubscription = this.usersService
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
      });
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }
}
