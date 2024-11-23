export class User {
  constructor(
    public id: string,
    public username: string,
    public email: string,
    public role: string,
    public created: Date
  ) {}
}
