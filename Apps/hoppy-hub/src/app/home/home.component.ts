import { Component } from '@angular/core';
import { MonthlyDataComponent } from './monthly-data/monthly-data.component';
import { RecentOpinionsComponent } from './recent-opinions/recent-opinions.component';
import { RecentBeersComponent } from './recent-beers/recent-beers.component';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  imports: [MonthlyDataComponent, RecentOpinionsComponent, RecentBeersComponent]
})
export class HomeComponent {}
