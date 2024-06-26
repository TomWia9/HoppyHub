import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { LoginModalComponent } from './auth/login-modal/login-modal.component';
import { RegisterModalComponent } from './auth/register-modal/register-modal.component';
import { AlertComponent } from './shared-components/alert/alert.component';
import { AuthService } from './auth/auth.service';
import { BeersFiltersModalComponent } from './beers/beers-table/beers-table-filters/beers-filters-modal/beers-filters-modal.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  imports: [
    CommonModule,
    RouterOutlet,
    HeaderComponent,
    FooterComponent,
    LoginModalComponent,
    RegisterModalComponent,
    AlertComponent,
    BeersFiltersModalComponent,
    NotFoundComponent
  ]
})
export class AppComponent implements OnInit {
  authService: AuthService = inject(AuthService);

  ngOnInit(): void {
    console.log('Production: ' + environment.production);
    this.authService.autoLogin();
  }
}
