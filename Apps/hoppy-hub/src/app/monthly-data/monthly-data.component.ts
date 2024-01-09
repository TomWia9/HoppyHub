import { Component } from '@angular/core';

@Component({
  selector: 'app-monthly-data',
  standalone: true,
  imports: [],
  templateUrl: './monthly-data.component.html',
  styleUrl: './monthly-data.component.css'
})
export class MonthlyDataComponent {
  totalBeersRated: number = 135;
  theMostPopularBrewery: string = 'Recraft';
  theMostActiveUser: string = 'TestUser';
  theMostActiveUserBeers: number = 31;
}
