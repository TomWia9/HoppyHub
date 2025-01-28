export class UpsertBreweryCommand {
  constructor(
    public id: string | null,
    public name: string,
    public description: string,
    public foundationYear: number,
    public websiteUrl: string,
    public street: string,
    public number: string,
    public postCode: string,
    public city: string,
    public state: string,
    public country: string
  ) {}
}
