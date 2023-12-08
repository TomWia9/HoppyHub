DECLARE @UserName NVARCHAR(256) = 'user@localhost';
DECLARE @UserId UNIQUEIDENTIFIER

-- Select Id for "user@localhost" user
SELECT @UserId = Id
FROM UserManagement.dbo.AspNetUsers 
WHERE UserName = @UserName

INSERT INTO Users (Id, Username, Role, Created, CreatedBy, LastModified, LastModifiedBy)
VALUES (@UserId, @UserName, 'User', GETDATE(), @UserId, GETDATE(), @UserId)