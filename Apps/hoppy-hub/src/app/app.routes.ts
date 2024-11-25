import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { BreweriesComponent } from './breweries/breweries.component';
import { BreweryDetailsComponent } from './breweries/brewery-details/brewery-details.component';
import { BeerDetailsComponent } from './beers/beer-details/beer-details.component';
import { BeersComponent } from './beers/beers.component';
import { UserDetailsComponent } from './users/user-details/user-details.component';

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
  {
    path: 'beers',
    component: BeersComponent,
    children: [{ path: ':id', component: BeerDetailsComponent }]
  },
  {
    path: 'users/:id',
    component: UserDetailsComponent
  },
  { path: 'not-found', component: NotFoundComponent },
  { path: '**', redirectTo: '/not-found' }
];
