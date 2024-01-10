import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { Subscription } from 'rxjs';
import { Opinion } from '../opinions/opinion.model';
import { OpinionsParams } from '../opinions/opinions-params';
import { OpinionsService } from '../services/opinions.service';
import { PagedList } from '../shared/paged-list';

@Component({
  selector: 'app-monthly-data',
  standalone: true,
  imports: [],
  templateUrl: './monthly-data.component.html',
  styleUrl: './monthly-data.component.css'
})
export class MonthlyDataComponent implements OnInit, OnDestroy {
  private opinionsService: OpinionsService = inject(OpinionsService);
  monthName!: string;
  totalBeersRated!: number;
  theMostPopularBeer: string = 'Recraft';
  theMostActiveUser: string = 'TestUser';
  theMostActiveUserBeers: number = 31;

  opinions!: PagedList<Opinion>;
  error = '';
  loading = false;
  opinionsChangedSubscription!: Subscription;
  errorSubscription!: Subscription;
  loadingSubscription!: Subscription;

  ngOnInit() {
    this.monthName = this.getPreviousMonthName();
    this.loadingSubscription = this.opinionsService.loading.subscribe(
      isLoading => {
        this.loading = isLoading;
      }
    );

    this.opinionsService.getOpinions(
      new OpinionsParams(10000, 1, 'lastModified', 1)
    );

    this.opinionsChangedSubscription =
      this.opinionsService.opinionsChanged.subscribe(opinions => {
        this.getTotalBeersRatedInLastMonth(opinions);
      });

    this.errorSubscription = this.opinionsService.errorCatched.subscribe(
      errorMessage => {
        this.error = errorMessage;
      }
    );
  }

  getTotalBeersRatedInLastMonth(opinions: PagedList<Opinion>): void {
    const currentDate = new Date();
    const lastMonthDate = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth() - 1,
      currentDate.getDate()
    );

    let totalBeersRated = 0;

    opinions.items.forEach(opinion => {
      const opinionDate = new Date(opinion.created);
      if (
        opinionDate.getFullYear() === lastMonthDate.getFullYear() &&
        opinionDate.getMonth() === lastMonthDate.getMonth()
      ) {
        totalBeersRated++;
      }
    });

    this.totalBeersRated = totalBeersRated;
  }

  getPreviousMonthName(): string {
    const months = [
      'January',
      'February',
      'March',
      'April',
      'May',
      'June',
      'July',
      'August',
      'September',
      'October',
      'November',
      'December'
    ];

    const currentDate = new Date();
    const previousMonthIndex = (currentDate.getMonth() - 1 + 12) % 12;

    return months[previousMonthIndex];
  }

  ngOnDestroy(): void {
    this.opinionsChangedSubscription.unsubscribe();
    this.errorSubscription.unsubscribe();
    this.loadingSubscription.unsubscribe();
  }
}
