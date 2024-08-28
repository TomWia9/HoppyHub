import { Component, Input } from '@angular/core';
import { Beer } from '../../../beers/beer.model';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-recent-beer',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './recent-beer.component.html'
})
export class RecentBeerComponent {
  @Input({ required: true }) beer!: Beer;
}
