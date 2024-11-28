export class DeleteUserCommand {
  constructor(
    public userId: string | null,
    public password: string
  ) {}
}
