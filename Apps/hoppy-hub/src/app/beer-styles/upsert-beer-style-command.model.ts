export class UpsertBeerStyleCommand {
  constructor(
    public id: string | null,
    public name: string,
    public description: string,
    public countryOfOrigin: string
  ) {}
}
