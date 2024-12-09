import { Beer } from '../beers/beer.model';
import { Brewery } from '../breweries/brewery.model';
import { User } from '../users/user.model';

export interface SearchResult {
  beers: Beer[];
  breweries: Brewery[];
  users: User[];
}
