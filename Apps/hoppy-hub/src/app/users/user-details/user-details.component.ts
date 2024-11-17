import { Component } from '@angular/core';
import { UserInfoComponent } from './user-info/user-info.component';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [UserInfoComponent],
  templateUrl: './user-details.component.html'
})
export class UserDetailsComponent {}
