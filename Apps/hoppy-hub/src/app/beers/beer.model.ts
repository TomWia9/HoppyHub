import { BeerStyle } from '../beer-styles/beer-style.model';
import { Brewery } from '../breweries/brewery.model';

export class Beer {
  constructor(
    public id: string,
    public name: string,
    public brewery: Brewery,
    public alcoholByVolume: number,
    public description: string,
    public composition: string,
    public blg: number,
    public rating: number,
    public beerStyle: BeerStyle,
    public ibu: number,
    public releaseDate: Date,
    public imageUri: string,
    public opinionsCount: number,
    public favoritesCount: number
  ) {}
}
