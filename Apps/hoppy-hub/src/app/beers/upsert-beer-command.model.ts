export class UpsertBeerCommand {
  constructor(
    public id: string | null,
    public name: string,
    public breweryId: string,
    public alcoholByVolume: number,
    public description: string,
    public composition: string,
    public blg: string,
    public beerStyleId: string,
    public ibu: number,
    public releaseDate: Date
  ) {}
}
