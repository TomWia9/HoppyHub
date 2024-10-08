import { Component, Input, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { RouterModule } from '@angular/router';
import { Beer } from '../../../beers/beer.model';
import { BeersService } from '../../../beers/beers.service';
import { Opinion } from '../../../opinions/opinion.model';

@Component({
  selector: 'app-recent-opinion',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './recent-opinion.component.html'
})
export class RecentOpinionComponent implements OnInit, OnDestroy {
  @Input({ required: true }) opinion!: Opinion;
  private beersService: BeersService = inject(BeersService);
  beer!: Beer;
  beerSubscription!: Subscription;

  ngOnInit(): void {
    this.beerSubscription = this.beersService
      .getBeerById(this.opinion.beerId)
      .subscribe((beer: Beer) => {
        this.beer = beer;
      });
  }

  getStars(rating: number): number[] {
    return Array.from({ length: rating }, (_, index) => index + 1);
  }

  getEmptyStars(rating: number): number[] {
    return Array(10 - rating).fill(0);
  }

  ngOnDestroy(): void {
    this.beerSubscription.unsubscribe();
  }
}
