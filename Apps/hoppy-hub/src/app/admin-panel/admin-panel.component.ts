import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './admin-panel.component.html'
})
export class AdminPanelComponent {}
