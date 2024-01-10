import { BreweryAddress } from './brewery-address.model';

export class Brewery {
  constructor(
    public id: string,
    public name: string,
    public description: string,
    public foundationYear: number,
    public websiteUrl: string,
    public address: BreweryAddress
  ) {}
}
