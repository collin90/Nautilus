CREATE OR ALTER PROCEDURE auth.LoginUser 
	@UserNameOrEmail NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserName,
        Email,
        PasswordHash,
        PasswordSalt,
        IsActive
    FROM auth.Users
    WHERE (UserName = @UserNameOrEmail OR Email = @UserNameOrEmail)
      AND IsActive = 1;
END
GO