-- Default password for Administrator: Admin123!
-- Default password for User: User123!

-- Roles ids
DECLARE @AdministratorRoleId NVARCHAR(36) = NEWID();
DECLARE @UserRoleId NVARCHAR(36) = NEWID();

-- Users ids
DECLARE @AdministratorId NVARCHAR(36) = NEWID();
DECLARE @UserId NVARCHAR(36) = NEWID();

INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (@AdministratorRoleId, 'Administrator', 'ADMINISTRATOR', null),
       (@UserRoleId, 'User', 'USER', null);

INSERT INTO AspNetUsers (Id ,UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount)  
VALUES (@AdministratorId, 'administrator@localhost', 'ADMINISTRATOR@LOCALHOST', 'administrator@localhost', 'ADMINISTRATOR@LOCALHOST', 0, 'AQAAAAIAAYagAAAAEEVAcxb2uPVZL/47GoN+DNtkUWV850gLE5GvcEWccGowDlUBUemyZwidTW/c3AuNfA==', 'WLDH4W3Q3NF44IGBRWZOM6JTEBRVG2HO', '28a3bea5-2c46-4464-876a-f6d88558f139', null, 0, 0, null, 1, 0),
       (@UserId, 'user@localhost', 'USER@LOCALHOST', 'user@localhost', 'USER@LOCALHOST', 0, 'AQAAAAIAAYagAAAAEB9+ccObuNAy6NKKkmetFNTcQsRNBTTgExfssos21PlHRbOJfc75QsDk60qxHTQ9Xg==', 'HQJE6CKX2EDPDPAH447PY36B7RGJT6SG', 'f4c1a047-7723-47a5-8888-51d1a87a8063', null, 0, 0, null, 1, 0);

INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES (@AdministratorId, @AdministratorRoleId),
       (@UserId, @UserRoleId);