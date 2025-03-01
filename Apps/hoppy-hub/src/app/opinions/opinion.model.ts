export class Opinion {
  constructor(
    public id: string,
    public rating: number,
    public comment: string,
    public imageUri: string,
    public beerId: string,
    public createdBy: string,
    public username: string,
    public userDeleted: boolean,
    public created: Date,
    public lastModified: Date
  ) {}
}
