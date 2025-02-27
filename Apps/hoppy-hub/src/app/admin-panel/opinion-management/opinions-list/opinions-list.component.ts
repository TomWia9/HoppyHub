import { Component } from '@angular/core';
import { OpinionsListFiltersComponent } from './opinions-list-filters/opinions-list-filters.component';

@Component({
  selector: 'app-opinions-list',
  standalone: true,
  imports: [OpinionsListFiltersComponent],
  templateUrl: './opinions-list.component.html'
})
export class OpinionsListComponent {}
