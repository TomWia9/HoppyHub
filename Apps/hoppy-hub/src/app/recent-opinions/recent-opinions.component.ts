import { Component } from '@angular/core';
import { RecentOpinionComponent } from './recent-opinion/recent-opinion.component';

@Component({
  selector: 'app-recent-opinions',
  standalone: true,
  templateUrl: './recent-opinions.component.html',
  styleUrl: './recent-opinions.component.css',
  imports: [RecentOpinionComponent]
})
export class RecentOpinionsComponent {}
