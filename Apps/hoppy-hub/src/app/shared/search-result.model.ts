import { Beer } from '../beers/beer.model';
import { Brewery } from '../breweries/brewery.model';

export interface SearchResult {
  beers: Beer[];
  breweries: Brewery[];
}
