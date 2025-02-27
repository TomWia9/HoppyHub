import { Component } from '@angular/core';
import { OpinionsListFiltersComponent } from './opinions-list-filters/opinions-list-filters.component';
import { OpinionsListComponent } from './opinions-list/opinions-list.component';

@Component({
  selector: 'app-opinion-management',
  standalone: true,
  imports: [OpinionsListFiltersComponent, OpinionsListComponent],
  templateUrl: './opinion-management.component.html'
})
export class OpinionManagementComponent {}
