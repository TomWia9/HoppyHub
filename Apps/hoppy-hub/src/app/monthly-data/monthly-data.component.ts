import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { Subscription } from 'rxjs';
import { Opinion } from '../opinions/opinion.model';
import { OpinionsParams } from '../opinions/opinions-params';
import { OpinionsService } from '../opinions/opinions.service';
import { PagedList } from '../shared/paged-list';
import { BeersService } from '../beers/beers.service';
import { Beer } from '../beers/beer.model';
import { User } from '../users/user.model';
import { UsersService } from '../users/users.service';

@Component({
  selector: 'app-monthly-data',
  standalone: true,
  imports: [],
  templateUrl: './monthly-data.component.html',
  styleUrl: './monthly-data.component.css'
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
        this.getLastMonthData(opinions);
      });

    this.errorSubscription = this.opinionsService.errorCatched.subscribe(
      errorMessage => {
        this.error = errorMessage;
      }
    );
  }

  getLastMonthData(opinions: PagedList<Opinion>): void {
    const currentDate = new Date();
    const lastMonthDate = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth() - 1,
      currentDate.getDate()
    );

    const opinionsFromLastMonth: Opinion[] = [];

    opinions.items.forEach(opinion => {
      const opinionDate = new Date(opinion.created);
      if (
        opinionDate.getFullYear() === lastMonthDate.getFullYear() &&
        opinionDate.getMonth() === lastMonthDate.getMonth()
      ) {
        opinionsFromLastMonth.push(opinion);
      }
    });

    const beerIdFrequencyMap = new Map<string, number>();
    const createdByFrequencyMap = new Map<string, number>();

    opinionsFromLastMonth.forEach(opinion => {
      const beerId = opinion.beerId;
      const createdBy = opinion.createdBy;
      const beerCount = beerIdFrequencyMap.get(beerId) || 0;
      const createdByCount = createdByFrequencyMap.get(createdBy) || 0;

      beerIdFrequencyMap.set(beerId, beerCount + 1);
      createdByFrequencyMap.set(createdBy, createdByCount + 1);
    });

    let maxBeerIdOccurrences = 0;
    let mostFrequentBeerId: string | null = null;

    beerIdFrequencyMap.forEach((occurrences, beerId) => {
      if (occurrences > maxBeerIdOccurrences) {
        maxBeerIdOccurrences = occurrences;
        mostFrequentBeerId = beerId;
      }
    });

    let maxCreatedByOccurrences = 0;
    let mostFrequentCreatedBy: string | null = null;

    createdByFrequencyMap.forEach((occurrences, createdBy) => {
      if (occurrences > maxCreatedByOccurrences) {
        maxCreatedByOccurrences = occurrences;
        mostFrequentCreatedBy = createdBy;
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

    if (mostFrequentCreatedBy) {
      this.usersService
        .getUserById(mostFrequentCreatedBy)
        .subscribe((user: User) => {
          this.theMostActiveUser = user;
        });
    } else {
      this.theMostActiveUser = null;
    }

    const theMostActiveUserOpinionsCount =
      createdByFrequencyMap.get(mostFrequentCreatedBy || '') || 0;

    this.totalBeersRated = opinionsFromLastMonth.length;
    this.theMostActiveUserBeersOpinionsCount = theMostActiveUserOpinionsCount;
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
