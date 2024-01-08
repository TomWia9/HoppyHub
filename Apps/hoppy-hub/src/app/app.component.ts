import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { LoginModalComponent } from './auth/login-modal/login-modal.component';
import { RegisterModalComponent } from './auth/register-modal/register-modal.component';
import { AlertComponent } from './alert/alert.component';

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  imports: [
    CommonModule,
    RouterOutlet,
    HeaderComponent,
    FooterComponent,
    LoginModalComponent,
    RegisterModalComponent,
    AlertComponent
  ]
})
export class AppComponent {}
