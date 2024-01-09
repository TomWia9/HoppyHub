import { Component } from '@angular/core';
import { MonthlyDataComponent } from '../monthly-data/monthly-data.component';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [MonthlyDataComponent]
})
export class HomeComponent {}
