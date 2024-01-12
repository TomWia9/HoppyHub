import { Component, Input } from '@angular/core';
import { Opinion } from '../../opinions/opinion.model';

@Component({
  selector: 'app-recent-opinion',
  standalone: true,
  imports: [],
  templateUrl: './recent-opinion.component.html',
  styleUrl: './recent-opinion.component.css'
})
export class RecentOpinionComponent {
  @Input({ required: true }) opinion!: Opinion;
}
