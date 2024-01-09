export class User {
  constructor(
    public id: string,
    public email: string,
    public role: string,
    private _token: string,
    public tokenExpirationDate: Date
  ) {}

  get token(): string | null {
    if (!this.tokenExpirationDate || new Date() > this.tokenExpirationDate) {
      return null;
    }
    return this._token;
  }
}
