export class BreweryAddress {
  constructor(
    public id: string,
    public street: string,
    public number: string,
    public postCode: string,
    public city: string,
    public state: string,
    public country: string
  ) {}
}
