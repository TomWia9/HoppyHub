import { inject, Injectable } from '@angular/core';
import { BeersService } from '../beers/beers.service';
import { BreweriesService } from '../breweries/breweries.service';
import { forkJoin, map, Observable } from 'rxjs';
import { BeersParams } from '../beers/beers-params';
import { BreweriesParams } from '../breweries/breweries-params';
import { SearchResult } from '../shared/search-result.model';
import { UsersService } from '../users/users.service';
import { UsersParams } from '../users/users-params';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private beersService: BeersService = inject(BeersService);
  private breweriesService: BreweriesService = inject(BreweriesService);
  private usersService: UsersService = inject(UsersService);

  searchGlobal(query: string): Observable<SearchResult> {
    return forkJoin({
      beers: this.beersService.getBeers(
        new BeersParams({ pageSize: 5, searchQuery: query })
      ),
      breweries: this.breweriesService.getBreweries(
        new BreweriesParams({ pageSize: 5, searchQuery: query })
      ),
      users: this.usersService.getUsers(
        new UsersParams({ pageSize: 5, searchQuery: query, role: 'user' })
      )
    }).pipe(
      map(({ beers, breweries, users }) => ({
        beers: beers.items,
        breweries: breweries.items,
        users: users.items
      }))
    );
  }
}
