import { Component } from '@angular/core';
import { MonthlyDataComponent } from '../monthly-data/monthly-data.component';
import { RecentOpinionsComponent } from '../recent-opinions/recent-opinions.component';
import { BeersTableComponent } from '../beers/beers-table/beers-table.component';
import { BeersFiltersModalComponent } from '../beers/beers-table/beers-table-filters/beers-filters-modal/beers-filters-modal.component';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  imports: [
    MonthlyDataComponent,
    RecentOpinionsComponent,
    BeersTableComponent,
    BeersFiltersModalComponent
  ]
})
export class HomeComponent {}
