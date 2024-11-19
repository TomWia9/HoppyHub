import { Component, Input } from '@angular/core';
import { Beer } from '../beer.model';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-beer-card',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './beer-card.component.html'
})
export class BeerCardComponent {
  @Input({ required: true }) beer!: Beer;
}
