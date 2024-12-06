import { inject, Injectable } from '@angular/core';
import { BeersService } from '../beers/beers.service';
import { BreweriesService } from '../breweries/breweries.service';
import { forkJoin, map, Observable } from 'rxjs';
import { BeersParams } from '../beers/beers-params';
import { BreweriesParams } from '../breweries/breweries-params';
import { SearchResult } from '../shared/search-result.model';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private beersService: BeersService = inject(BeersService);
  private breweriesService: BreweriesService = inject(BreweriesService);

  searchGlobal(query: string): Observable<SearchResult> {
    return forkJoin({
      beers: this.beersService.getBeers(
        new BeersParams({ searchQuery: query })
      ),
      breweries: this.breweriesService.getBreweries(
        new BreweriesParams({ searchQuery: query })
      )
    }).pipe(
      map(({ beers, breweries }) => ({
        beers: beers.items,
        breweries: breweries.items
      }))
    );
  }
}
