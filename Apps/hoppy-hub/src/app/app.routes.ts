import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { BreweriesComponent } from './breweries/breweries.component';
import { BreweryDetailsComponent } from './breweries/brewery-details/brewery-details.component';
import { BeerDetailsComponent } from './beers/beer-details/beer-details.component';
import { BeersComponent } from './beers/beers.component';
import { UserDetailsComponent } from './users/user-details/user-details.component';
import { AccountSettingsComponent } from './users/account-settings/account-settings.component';
import { AboutComponent } from './about/about.component';
import { BeerStylesComponent } from './beer-styles/beer-styles.component';
import { BeerStyleDetailsComponent } from './beer-styles/beer-style-details/beer-style-details.component';

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
    path: 'beer-styles',
    component: BeerStylesComponent,
    children: [{ path: ':id', component: BeerStyleDetailsComponent }]
  },
  {
    path: 'users/:id',
    component: UserDetailsComponent
  },
  {
    path: 'account-settings',
    component: AccountSettingsComponent
  },
  { path: 'about', component: AboutComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: '**', redirectTo: '/not-found' }
];
