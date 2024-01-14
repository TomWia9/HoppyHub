import { Component, Input } from '@angular/core';
import { Opinion } from '../../opinions/opinion.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-recent-opinion',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './recent-opinion.component.html',
  styleUrl: './recent-opinion.component.css'
})
export class RecentOpinionComponent {
  @Input({ required: true }) opinion!: Opinion;

  getStars(rating: number): number[] {
    return Array.from({ length: rating }, (_, index) => index + 1);
  }
}
