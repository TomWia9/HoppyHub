import { Component } from '@angular/core';
import { BreweriesTableComponent } from './breweries-table/breweries-table.component';
import { BreweryDetailsComponent } from './brewery-details/brewery-details.component';

@Component({
  selector: 'app-breweries',
  standalone: true,
  templateUrl: './breweries.component.html',
  imports: [BreweriesTableComponent, BreweryDetailsComponent]
})
export class BreweriesComponent {}
