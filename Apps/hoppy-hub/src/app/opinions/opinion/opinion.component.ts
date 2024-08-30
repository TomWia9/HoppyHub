import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Beer } from '../../beers/beer.model';
import { Opinion } from '../opinion.model';

@Component({
  selector: 'app-opinion',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './opinion.component.html'
})
export class OpinionComponent {
  @Input({ required: true }) beer!: Beer;
  @Input({ required: true }) opinion!: Opinion;

  getStars(rating: number): number[] {
    return Array.from({ length: rating }, (_, index) => index + 1);
  }

  getEmptyStars(rating: number): number[] {
    return Array(10 - rating).fill(0);
  }
}
