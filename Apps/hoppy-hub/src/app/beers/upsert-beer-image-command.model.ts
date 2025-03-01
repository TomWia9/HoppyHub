export class UpsertBeerImageCommand {
  constructor(
    public beerId: string,
    public image: File | null
  ) {}
}
