import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { Subscription } from 'rxjs';
import { RouterModule } from '@angular/router';
import { Beer } from '../../beers/beer.model';
import { BeersService } from '../../beers/beers.service';
import { Opinion } from '../../opinions/opinion.model';
import { OpinionsParams } from '../../opinions/opinions-params';
import { OpinionsService } from '../../opinions/opinions.service';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { PagedList } from '../../shared/paged-list';
import { User } from '../../users/user.model';
import { UsersService } from '../../users/users.service';

@Component({
  selector: 'app-monthly-data',
  standalone: true,
  templateUrl: './monthly-data.component.html',
  imports: [LoadingSpinnerComponent, ErrorMessageComponent, RouterModule]
})
export class MonthlyDataComponent implements OnInit, OnDestroy {
  private opinionsService: OpinionsService = inject(OpinionsService);
  private beersService: BeersService = inject(BeersService);
  private usersService: UsersService = inject(UsersService);

  monthName!: string;
  totalBeersRated!: number;
  theMostPopularBeer!: Beer | null;
  theMostActiveUser!: User | null;
  theMostActiveUserBeersOpinionsCount!: number;
  error = '';
  loading = true;
  getOpinionsSubscription!: Subscription;

  ngOnInit(): void {
    this.monthName = this.getPreviousMonthName();
    this.fetchAllOpinions();
  }

  private fetchAllOpinions(
    pageNumber: number = 1,
    allOpinions: Opinion[] = []
  ): void {
    const { from, to } = this.getPreviousMonthRange();

    this.getOpinionsSubscription = this.opinionsService
      .getOpinions(
        new OpinionsParams({
          pageSize: 50,
          pageNumber: pageNumber,
          sortBy: 'lastModified',
          sortDirection: 1,
          from: from.toDateString(),
          to: to.toDateString()
        })
      )
      .subscribe({
        next: (opinions: PagedList<Opinion>) => {
          this.loading = true;
          allOpinions.push(...opinions.items);
          if (opinions.HasNext) {
            this.fetchAllOpinions(pageNumber + 1, allOpinions);
          } else {
            this.getLastMonthData(allOpinions);
            this.error = '';
            this.loading = false;
          }
        },
        error: () => {
          this.error = 'An error occurred while loading monthly data';
          this.loading = false;
        }
      });
  }

  private getPreviousMonthRange(): { from: Date; to: Date } {
    const today = new Date();
    let year = today.getFullYear();
    let month = today.getMonth();

    if (month === 0) {
      year--;
      month = 11;
    } else {
      month--;
    }

    const firstDayOfPreviousMonth = new Date(year, month, 1);
    const lastDayOfPreviousMonth = new Date(
      today.getFullYear(),
      today.getMonth(),
      0
    );

    return {
      from: firstDayOfPreviousMonth,
      to: lastDayOfPreviousMonth
    };
  }

  private getLastMonthData(opinions: Opinion[]): void {
    this.totalBeersRated = opinions.length;
    this.setTheMostPopularBeer(opinions);
    this.setTheMostActiveUser(opinions);
  }

  private setTheMostPopularBeer(opinions: Opinion[]): void {
    const beerIdFrequencyMap = new Map<string, number>();

    opinions.forEach(opinion => {
      const beerId = opinion.beerId;
      const beerCount = beerIdFrequencyMap.get(beerId) || 0;

      beerIdFrequencyMap.set(beerId, beerCount + 1);
    });

    let maxBeerIdOccurrences = 0;
    let mostFrequentBeerId: string | null = null;

    beerIdFrequencyMap.forEach((occurrences, beerId) => {
      if (occurrences > maxBeerIdOccurrences) {
        maxBeerIdOccurrences = occurrences;
        mostFrequentBeerId = beerId;
      }
    });

    if (mostFrequentBeerId) {
      this.beersService
        .getBeerById(mostFrequentBeerId)
        .subscribe((beer: Beer) => {
          this.theMostPopularBeer = beer;
        });
    } else {
      this.theMostPopularBeer = null;
    }
  }

  private setTheMostActiveUser(opinions: Opinion[]): void {
    const createdByFrequencyMap = new Map<string, number>();

    opinions.forEach(opinion => {
      const createdBy = opinion.createdBy;
      const createdByCount = createdByFrequencyMap.get(createdBy) || 0;

      createdByFrequencyMap.set(createdBy, createdByCount + 1);
    });

    let maxCreatedByOccurrences = 0;
    let mostFrequentCreatedBy: string | null = null;

    createdByFrequencyMap.forEach((occurrences, createdBy) => {
      if (occurrences > maxCreatedByOccurrences) {
        maxCreatedByOccurrences = occurrences;
        mostFrequentCreatedBy = createdBy;
      }
    });

    if (mostFrequentCreatedBy) {
      this.usersService
        .getUserById(mostFrequentCreatedBy)
        .subscribe((user: User) => {
          const theMostActiveUserOpinionsCount =
            createdByFrequencyMap.get(mostFrequentCreatedBy || '') || 0;
          this.theMostActiveUserBeersOpinionsCount =
            theMostActiveUserOpinionsCount;
          this.theMostActiveUser = user;
        });
    } else {
      this.theMostActiveUser = null;
    }
  }

  private getPreviousMonthName(): string {
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
    if (this.getOpinionsSubscription) {
      this.getOpinionsSubscription.unsubscribe();
    }
  }
}
