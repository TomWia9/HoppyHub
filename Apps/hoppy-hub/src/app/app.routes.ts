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
import { AdminPanelComponent } from './admin-panel/admin-panel.component';
import { authGuard } from './auth/auth-guard';
import { BeerManagementComponent } from './admin-panel/beer-management/beer-management.component';
import { Roles } from './auth/roles';
import { NewBeerComponent } from './admin-panel/beer-management/new-beer/new-beer.component';
import { EditBeerComponent } from './admin-panel/beer-management/edit-beer/edit-beer.component';
import { BreweryManagementComponent } from './admin-panel/brewery-management/brewery-management.component';
import { NewBreweryComponent } from './admin-panel/brewery-management/new-brewery/new-brewery.component';
import { EditBreweryComponent } from './admin-panel/brewery-management/edit-brewery/edit-brewery.component';
import { BeerStyleManagementComponent } from './admin-panel/beer-style-management/beer-style-management.component';
import { NewBeerStyleComponent } from './admin-panel/beer-style-management/new-beer-style/new-beer-style.component';
import { EditBeerStyleComponent } from './admin-panel/beer-style-management/edit-beer-style/edit-beer-style.component';
import { OpinionManagementComponent } from './admin-panel/opinion-management/opinion-management.component';

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
  {
    path: 'admin-panel',
    component: AdminPanelComponent,
    canActivate: [authGuard],
    data: { roles: [Roles.Administrator] },
    children: [
      { path: '', redirectTo: 'beer-management', pathMatch: 'full' },
      {
        path: 'beer-management',
        component: BeerManagementComponent,
        children: [
          { path: 'new', component: NewBeerComponent },
          { path: ':id', component: EditBeerComponent }
        ]
      },
      {
        path: 'brewery-management',
        component: BreweryManagementComponent,
        children: [
          { path: 'new', component: NewBreweryComponent },
          { path: ':id', component: EditBreweryComponent }
        ]
      },
      {
        path: 'beer-style-management',
        component: BeerStyleManagementComponent,
        children: [
          { path: 'new', component: NewBeerStyleComponent },
          { path: ':id', component: EditBeerStyleComponent }
        ]
      },
      {
        path: 'opinion-management',
        component: OpinionManagementComponent
      }
    ]
  },
  { path: 'about', component: AboutComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: '**', redirectTo: '/not-found' }
];
