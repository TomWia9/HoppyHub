import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { BreweriesComponent } from './breweries/breweries.component';
import { BreweryDetailsComponent } from './breweries/brewery-details/brewery-details.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'breweries',
    component: BreweriesComponent,
    children: [{ path: ':id', component: BreweryDetailsComponent }]
  },
  { path: 'not-found', component: NotFoundComponent },
  { path: '**', redirectTo: '/not-found' }
];
