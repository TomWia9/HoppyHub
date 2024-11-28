export class UpdateUserPasswordCommand {
  constructor(
    public userId: string | null,
    public currentPassword: string,
    public newPassword: string
  ) {}
}
