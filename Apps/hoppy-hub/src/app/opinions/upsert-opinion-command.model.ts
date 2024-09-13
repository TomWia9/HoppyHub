export class UpsertOpinionCommand {
  constructor(
    public id: string | null,
    public beerId: string,
    public rating: number,
    public comment: string,
    public image: File | null,
    public deleteImage: boolean
  ) {}
}
