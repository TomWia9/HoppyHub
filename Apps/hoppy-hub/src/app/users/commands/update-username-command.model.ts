export class UpdateUsernameCommand {
  constructor(
    public userId: string | null,
    public username: string
  ) {}
}
