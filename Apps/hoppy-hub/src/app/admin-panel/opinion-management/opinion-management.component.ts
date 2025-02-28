import { Component } from '@angular/core';
import { OpinionsListComponent } from './opinions-list/opinions-list.component';

@Component({
  selector: 'app-opinion-management',
  standalone: true,
  imports: [OpinionsListComponent],
  templateUrl: './opinion-management.component.html'
})
export class OpinionManagementComponent {}
