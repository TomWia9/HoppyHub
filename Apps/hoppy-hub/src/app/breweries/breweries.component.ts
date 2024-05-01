import { Component } from '@angular/core';
import { BreweriesTableComponent } from './breweries-table/breweries-table.component';
import { BreweryDetailsComponent } from './brewery-details/brewery-details.component';
import { BreweriesFiltersModalComponent } from './breweries-table/breweries-table-filters/breweries-filters-modal/breweries-filters-modal.component';

@Component({
  selector: 'app-breweries',
  standalone: true,
  templateUrl: './breweries.component.html',
  imports: [
    BreweriesTableComponent,
    BreweryDetailsComponent,
    BreweriesFiltersModalComponent
  ]
})
export class BreweriesComponent {}
