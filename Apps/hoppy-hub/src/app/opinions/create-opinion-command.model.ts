export class CreateOpinionCommand {
  constructor(
    public beerId: string,
    public rating: number,
    public comment: string,
    public image: File | null
  ) {}
}
