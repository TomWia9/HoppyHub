import { Component } from '@angular/core';
import { MonthlyDataComponent } from '../monthly-data/monthly-data.component';
import { RecentOpinionsComponent } from '../recent-opinions/recent-opinions.component';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [MonthlyDataComponent, RecentOpinionsComponent]
})
export class HomeComponent {}
